#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.PaymentGateway;
using pwr_msi.Serialization;

namespace pwr_msi.Services {
    public class PaymentService {
        private readonly AppConfig _appConfig;
        private readonly MsiDbContext _dbContext;
        private readonly OrderTaskService _orderTaskService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PaymentService> _logger;

        private string _gatewayEndpointBase => _appConfig.PayUrl.TrimEnd('/') + '/';
        private string _gatewayEndpointNew => _gatewayEndpointBase + "api/new/";
        private string _gatewayEndpointInfo(string externalId) => _gatewayEndpointBase + $"api/payment/{externalId}";

        private static readonly JsonSerializerSettings _gatewayJsonSerializerSettings = new() {
            FloatParseHandling = FloatParseHandling.Decimal,
            ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
            Converters = new List<JsonConverter> {
                new StringEnumConverter(new UpperSnakeCaseNamingStrategy(), allowIntegerValues: false)
                },
        };

        public PaymentService(AppConfig appConfig, MsiDbContext dbContext, OrderTaskService orderTaskService,
            IHttpClientFactory httpClientFactory, ILogger<PaymentService> logger) {
            _appConfig = appConfig;
            _dbContext = dbContext;
            _orderTaskService = orderTaskService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<PaymentGroupInfo> CreatePaymentForOrder(Order order, bool useBalance = false) {
            PaymentGroupInfo paymentGroupInfo = null!;
            var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () => {
                await using var transaction =
                    await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                var alreadyPaid = await _dbContext.Payments
                    .Where(p => p.OrderId == order.OrderId && p.Status == PaymentStatus.COMPLETED)
                    .SumAsync(p => p.Amount);
                // ReSharper disable once ArgumentsStyleLiteral
                var remainingToPay = Math.Max(0, order.TotalPrice - alreadyPaid);

                decimal paidFromBalance = 0;
                if (useBalance) {
                    paidFromBalance = Math.Min(order.Customer.Balance, remainingToPay);
                }

                var paidExternally = remainingToPay - paidFromBalance;

                if (paidFromBalance > 0) {
                    var balancePayment = new Payment {
                        Amount = paidFromBalance,
                        OrderId = order.OrderId,
                        UserId = order.CustomerId,
                        Status = PaymentStatus.CREATED,
                        IsTargettingBalance = true,
                        Created = Utils.Now(),
                        Updated = Utils.Now()
                    };
                    await ProcessNewBalancePayment(balancePayment);
                }

                string? paymentLink = null;
                if (paidExternally > 0) {
                    var externalPayment = new Payment {
                        Amount = paidExternally,
                        OrderId = order.OrderId,
                        UserId = order.CustomerId,
                        Status = PaymentStatus.CREATED,
                        Created = Utils.Now(),
                        Updated = Utils.Now()
                    };
                    paymentLink = await ProcessNewExternalPayment(externalPayment);
                }

                var isPaid = paidExternally == 0;

                await transaction.CommitAsync();

                paymentGroupInfo = new PaymentGroupInfo {
                    PaidFromBalance = paidFromBalance,
                    PaidExternally = paidExternally,
                    IsPaid = isPaid,
                    PaymentLink = paymentLink,
                };
            });
            return paymentGroupInfo;
        }

        public async Task<Payment> RefreshStatusFromApi(Payment payment) {
            await GetPaymentInfoAndRefreshStatus(payment);
            return payment;
        }

        public async Task<PaymentAttemptDto> MakePayment(Payment payment) {
            if (!payment.CanPay) {
                return new PaymentAttemptDto {PaymentId = payment.PaymentId, PaymentStatus = payment.Status};
            }

            if (payment.IsTargettingBalance) {
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

        public async Task<Payment> ReturnOrderPayment(Order order) {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var amountToReturn = await _dbContext.Payments
                .Where(p => p.OrderId == order.OrderId && p.Status == PaymentStatus.COMPLETED).SumAsync(p => p.Amount);
            var payment = new Payment {
                Amount = -amountToReturn,
                OrderId = order.OrderId,
                UserId = order.CustomerId,
                Status = PaymentStatus.CREATED,
                IsTargettingBalance = true,
                Created = Utils.Now(),
                Updated = Utils.Now()
            };
            await ProcessNewBalancePayment(payment);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return payment;
        }

        public async Task<PaymentAttemptDto?> RepayBalance(int userId) {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var user = await _dbContext.Users.FindAsync(userId);
            var balance = user?.Balance ?? 0;
            if (user == null || balance <= 0) {
                await transaction.RollbackAsync();
                return null;
            }

            user.Balance -= balance;
            var payment = new Payment {
                Amount = -balance,
                UserId = user.UserId,
                Status = PaymentStatus.CREATED,
                IsBalanceRepayment = true,
                Created = Utils.Now(),
                Updated = Utils.Now()
            };
            var url = await ProcessNewExternalPayment(payment);
            await transaction.CommitAsync();
            return new PaymentAttemptDto {
                PaymentId = payment.PaymentId,
                PaymentStatus = payment.Status,
                PaymentUrl = url
            };
        }

        public async Task HandlePaymentCallback(int paymentId, PaymentStatus status, string? error) {
            _logger.LogDebug("Payment callback received for {PaymentId}", paymentId);
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var payment = await _dbContext.Payments.FindAsync(paymentId);
            var oldStatus = payment.Status;
            payment.Status = status;
            payment.ErrorMessage = error;
            await _dbContext.SaveChangesAsync();
            await HandlePaymentStatusUpdate(oldStatus, payment);
            await transaction.CommitAsync();
        }

        private async Task<string> ProcessNewExternalPayment(Payment payment) {
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            return await RegisterExternalPayment(payment);
        }

        private async Task<PaymentInfoDto> GetPaymentInfoAndRefreshStatus(Payment payment) {
            PaymentInfoDto paymentInfo = null!;
            var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () => {
                await using var transaction =
                    await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                var httpClient = _httpClientFactory.CreateClient();
                Debug.Assert(payment.ExternalPaymentId != null, "payment.ExternalPaymentId != null");
                using var httpResponse = await httpClient.GetAsync(_gatewayEndpointInfo(payment.ExternalPaymentId));
                paymentInfo = await DeserializeFromGateway<PaymentInfoDto>(httpResponse);
                await UpdatePaymentFromApi(payment, paymentInfo.Payment);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            });
            return paymentInfo;
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

            var user = await _dbContext.Users.FindAsync(payment.UserId);

            if (payment.IsReturn) {
                user.Balance += payment.AbsAmount;
                payment.Status = PaymentStatus.COMPLETED;
            } else if (user.Balance > payment.AbsAmount) {
                payment.Status = PaymentStatus.FAILED;
                payment.ErrorMessage = "INSUFFICIENT_BALANCE";
            } else {
                user.Balance -= payment.AbsAmount;
                payment.Status = PaymentStatus.COMPLETED;
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            if (payment.OrderId.HasValue) {
                await _orderTaskService.TryMarkAsPaid(payment.OrderId.Value);
            }
        }

        private async Task<PaymentInfoDto> MakeExternalPayment(Payment payment) {
            return await GetPaymentInfoAndRefreshStatus(payment);
        }

        private async Task UpdatePaymentFromApi(Payment payment, PaymentApiDto apiPayment) {
            var oldStatus = payment.Status;
            payment.UpdateFromApi(apiPayment);
            await _dbContext.SaveChangesAsync();
            await HandlePaymentStatusUpdate(oldStatus, payment);
        }

        private async Task HandlePaymentStatusUpdate(PaymentStatus oldStatus, Payment payment) {
            if (oldStatus != payment.Status && payment.Status == PaymentStatus.COMPLETED && payment.OrderId.HasValue) {
                await _orderTaskService.TryMarkAsPaid(payment.OrderId.Value);
            } else if (oldStatus != payment.Status && payment.IsUnsuccessful && payment.IsBalanceRepayment) {
                var user = await _dbContext.Users.FindAsync(payment.UserId);
                user.Balance += payment.AbsAmount;
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<string> RegisterExternalPayment(Payment payment) {
            _logger.LogDebug("Registering external payment for payment {PaymentId}", payment.PaymentId);
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
