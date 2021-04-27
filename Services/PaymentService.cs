#nullable enable
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
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
        private AppConfig _appConfig;
        private MsiDbContext _dbContext;
        private OrderTaskService _orderTaskService;
        private IHttpClientFactory _httpClientFactory;

        private string _gatewayEndpointBase => _appConfig.PayUrl.TrimEnd('/') + '/';
        private string _gatewayEndpointNew => _gatewayEndpointBase + "api/new/";
        private string _gatewayEndpointInfo(string externalId) => _gatewayEndpointBase + $"api/payment/{externalId}";
        private static JsonSerializerSettings _gatewayJsonSerializerSettings = new JsonSerializerSettings {
            FloatParseHandling = FloatParseHandling.Decimal,
            ContractResolver = new DefaultContractResolver() {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
        };

        public PaymentService(AppConfig appConfig, MsiDbContext dbContext, OrderTaskService orderTaskService, IHttpClientFactory httpClientFactory) {
            _appConfig = appConfig;
            _dbContext = dbContext;
            _orderTaskService = orderTaskService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PaymentGroupInfo> CreatePaymentForOrder(Order order, bool useBalance = false) {
            decimal paidFromBalance = 0;
            if (useBalance) {
                paidFromBalance = Math.Min(order.Customer.Balance, order.TotalPrice);
            }
            var paidExternally = order.TotalPrice - paidFromBalance;

            if (paidFromBalance > 0) {
                var balancePayment = new Payment {
                    Amount = paidFromBalance,
                    Order = order,
                    Status = PaymentStatus.CREATED,
                    IsFromBalance = true,
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
                PaidFromBalance = paidFromBalance, PaidExternally = paidExternally, IsPaid = isPaid, PaymentLink = paymentLink
            };
        }

        public async Task ProcessNewBalancePayment(Payment payment) {
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            await MakeBalancePayment(payment);
        }

        public async Task MakeBalancePayment(Payment payment) {
            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.Serializable);

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

        public async Task<PaymentInfoDto> MakeExternalPayment(Payment payment) {
            var httpClient = _httpClientFactory.CreateClient();
            Debug.Assert(payment.ExternalPaymentId != null, "payment.ExternalPaymentId != null");
            using var httpResponse = await httpClient.GetAsync(_gatewayEndpointInfo(payment.ExternalPaymentId));
            var paymentInfo = await DeserializeFromGateway<PaymentInfoDto>(httpResponse);
            await UpdatePaymentFromApi(payment, paymentInfo.Payment);
            await _dbContext.SaveChangesAsync();
            return paymentInfo;
        }

        private async Task UpdatePaymentFromApi(Payment payment, PaymentApiDto apiPayment) {
            var oldStatus = payment.Status;
            payment.UpdateFromApi(apiPayment);
            await _dbContext.SaveChangesAsync();
            if (oldStatus != payment.Status && payment.Status == PaymentStatus.COMPLETED) {
                await _orderTaskService.TryMarkAsPaid(payment.OrderId);
            }
        }

        public async Task<string> ProcessNewExternalPayment(Payment payment) {
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            return await RegisterExternalPayment(payment);
        }

        public async Task<string> RegisterExternalPayment(Payment payment) {
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

        public async Task<PaymentAttemptDto> MakePayment(Payment payment) {
            if (!payment.CanPay) {
                return new PaymentAttemptDto {PaymentStatus = payment.Status};
            }
            if (payment.IsFromBalance) {
                await MakeBalancePayment(payment);
                return new PaymentAttemptDto {PaymentStatus = payment.Status};
            }
            if (payment.Status == PaymentStatus.CREATED) {
                var url = await RegisterExternalPayment(payment);
                return new PaymentAttemptDto {PaymentStatus = payment.Status, PaymentUrl = url};
            }

            var paymentInfo = await MakeExternalPayment(payment);
            return new PaymentAttemptDto {PaymentStatus = payment.Status, PaymentUrl = paymentInfo.Url};
        }

        private async Task<T> DeserializeFromGateway<T>(HttpResponseMessage httpResponse) {
            httpResponse.EnsureSuccessStatusCode();
            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var jsonSerializer = JsonSerializer.Create(_gatewayJsonSerializerSettings);
            return jsonSerializer.Deserialize<T>(jsonTextReader);
        }
    }
}
