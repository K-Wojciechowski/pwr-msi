#nullable enable
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.PaymentGateway;

namespace pwr_msi.Services {
    public class PaymentService {
        private readonly AppConfig _appConfig;
        private readonly MsiDbContext _dbContext;
        private readonly OrderTaskService _orderTaskService;
        private readonly IHttpClientFactory _httpClientFactory;

        private string _gatewayEndpointBase => _appConfig.PayUrl.TrimEnd('/') + '/';
        private string _gatewayEndpointNew => _gatewayEndpointBase + "api/new/";
        private string _gatewayEndpointInfo(string externalId) => _gatewayEndpointBase + $"api/payment/{externalId}";

        private static readonly JsonSerializerSettings _gatewayJsonSerializerSettings = new() {
            FloatParseHandling = FloatParseHandling.Decimal,
            ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
        };

        public PaymentService(AppConfig appConfig, MsiDbContext dbContext, OrderTaskService orderTaskService,
            IHttpClientFactory httpClientFactory) {
            _appConfig = appConfig;
            _dbContext = dbContext;
            _orderTaskService = orderTaskService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PaymentGroupInfo> CreatePaymentForOrder(Order order, bool useBalance = false) {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var alreadyPaid = await _dbContext.Payments
                .Where(p => p.OrderId == order.OrderId && p.Status == PaymentStatus.COMPLETED).SumAsync(p => p.Amount);
            // ReSharper disable once ArgumentsStyleLiteral
            var remainingToPay = Math.Max(0, order.TotalPrice - alreadyPaid);

            decimal paidFromBalance = 0;
            if (useBalance) {
                paidFromBalance = Math.Min(order.Customer.Balance, remainingToPay);
            }

            var paidExternally = remainingToPay - paidFromBalance;

            if (paidFromBalance > 0) {
                var balancePayment = new Payment {
                    Amount = paidFromBalance, Order = order, Status = PaymentStatus.CREATED, IsFromBalance = true,
                };
                await ProcessNewBalancePayment(balancePayment);
            }

            string? paymentLink = null;
            if (paidExternally > 0) {
                var externalPayment = new Payment {
                    Amount = paidExternally, Order = order, Status = PaymentStatus.CREATED,
                };
                paymentLink = await ProcessNewExternalPayment(externalPayment);
            }

            var isPaid = paidExternally == 0;

            return new PaymentGroupInfo {
                PaidFromBalance = paidFromBalance,
                PaidExternally = paidExternally,
                IsPaid = isPaid,
                PaymentLink = paymentLink,
            };
        }

        public async Task<Payment> RefreshStatusFromApi(Payment payment) {
            await GetPaymentInfoAndRefreshStatus(payment);
            return payment;
        }

        public async Task<string> ProcessNewExternalPayment(Payment payment) {
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            return await RegisterExternalPayment(payment);
        }

        public async Task<PaymentInfoDto> GetPaymentInfoAndRefreshStatus(Payment payment) {
            var httpClient = _httpClientFactory.CreateClient();
            Debug.Assert(payment.ExternalPaymentId != null, "payment.ExternalPaymentId != null");
            using var httpResponse = await httpClient.GetAsync(_gatewayEndpointInfo(payment.ExternalPaymentId));
            var paymentInfo = await DeserializeFromGateway<PaymentInfoDto>(httpResponse);
            await UpdatePaymentFromApi(payment, paymentInfo.Payment);
            await _dbContext.SaveChangesAsync();
            return paymentInfo;
        }

        public async Task<PaymentAttemptDto> MakePayment(Payment payment) {
            if (!payment.CanPay) {
                return new PaymentAttemptDto {PaymentId = payment.PaymentId, PaymentStatus = payment.Status};
            }

            if (payment.IsFromBalance) {
                await MakeBalancePayment(payment);
                return new PaymentAttemptDto {PaymentId = payment.PaymentId, PaymentStatus = payment.Status};
            }

            if (payment.Status == PaymentStatus.CREATED) {
                var url = await RegisterExternalPayment(payment);
                return new PaymentAttemptDto {
                    PaymentId = payment.PaymentId, PaymentStatus = payment.Status, PaymentUrl = url
                };
            }

            var paymentInfo = await MakeExternalPayment(payment);
            return new PaymentAttemptDto {
                PaymentId = payment.PaymentId, PaymentStatus = payment.Status, PaymentUrl = paymentInfo.Url
            };
        }

        public async Task HandlePaymentCallback(Payment payment, PaymentStatus status, string? error) {
            payment.Status = status;
            payment.ErrorMessage = error;
            _dbContext.Attach(payment);
            await _dbContext.SaveChangesAsync();
            await _orderTaskService.TryMarkAsPaid(payment.OrderId);
        }

        private async Task ProcessNewBalancePayment(Payment payment) {
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            await MakeBalancePayment(payment);
        }

        private async Task MakeBalancePayment(Payment payment) {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            if (payment.Status != PaymentStatus.CREATED && payment.Status != PaymentStatus.REQUESTED) {
                throw new Exception("Invalid payment status");
            }

            var user = await _dbContext.Users.FindAsync(payment.Order.CustomerId);
            if (user.Balance > payment.Amount) {
                payment.Status = PaymentStatus.FAILED;
                payment.ErrorMessage = "INSUFFICIENT_BALANCE";
            } else {
                user.Balance -= payment.Amount;
                payment.Status = PaymentStatus.COMPLETED;
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            await _orderTaskService.TryMarkAsPaid(payment.OrderId);
        }

        private async Task<PaymentInfoDto> MakeExternalPayment(Payment payment) {
            return await GetPaymentInfoAndRefreshStatus(payment);
        }

        private async Task UpdatePaymentFromApi(Payment payment, PaymentApiDto apiPayment) {
            var oldStatus = payment.Status;
            payment.UpdateFromApi(apiPayment);
            await _dbContext.SaveChangesAsync();
            if (oldStatus != payment.Status && payment.Status == PaymentStatus.COMPLETED) {
                await _orderTaskService.TryMarkAsPaid(payment.OrderId);
            }
        }

        private async Task<string> RegisterExternalPayment(Payment payment) {
            var httpClient = _httpClientFactory.CreateClient();
            var paymentRequest = payment.AsRequestDto();
            var httpRequestContent = new StringContent(
                JsonConvert.SerializeObject(paymentRequest, _gatewayJsonSerializerSettings),
                Encoding.UTF8,
                mediaType: "application/json");
            using var httpResponse = await httpClient.PostAsync(_gatewayEndpointNew, httpRequestContent);
            var paymentInfo = await DeserializeFromGateway<PaymentInfoDto>(httpResponse);
            await UpdatePaymentFromApi(payment, paymentInfo.Payment);
            return paymentInfo.Url;
        }

        private static async Task<T> DeserializeFromGateway<T>(HttpResponseMessage httpResponse) {
            httpResponse.EnsureSuccessStatusCode();
            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var jsonSerializer = JsonSerializer.Create(_gatewayJsonSerializerSettings);
            return jsonSerializer.Deserialize<T>(jsonTextReader);
        }
    }
}
