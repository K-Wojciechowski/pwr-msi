#nullable enable
using System;
using NodaTime;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.PaymentGateway;

namespace pwr_msi.Models {
    public class Payment {
        public int PaymentId { get; set; }
        public string? ExternalPaymentId { get; set; }
        public bool IsReturn { get; set; }
        public bool IsBalanceRepayment { get; set; }
        public bool IsTargettingBalance { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? ErrorMessage { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; } = null!;

        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public bool CanPay => Status == PaymentStatus.CREATED || Status == PaymentStatus.REQUESTED;
        public decimal AbsAmount => Math.Abs(Amount);
        public bool IsUnsuccessful => Status == PaymentStatus.FAILED || Status == PaymentStatus.CANCELLED;

        private string PayerName => IsBalanceRepayment ? "SYS" : IsReturn ? User.FullName : Order?.Restaurant.Name ?? "?";
        private string PayeeName => IsBalanceRepayment || IsReturn ? User.FullName : Order?.Restaurant.Name ?? "?";
        private string Description => IsBalanceRepayment ? "Balance Repayment": IsReturn ? $"MSI order return {OrderId}" : $"MSI order {OrderId}";

        public PaymentRequestDto AsRequestDto() => new() {
            Amount = AbsAmount,
            Currency = Constants.DefaultCurrency,
            Payer = PayerName,
            Payee = PayeeName,
            IsReturn = IsBalanceRepayment,
            Description = Description,
        };

        public void UpdateFromApi(PaymentApiDto apiPayment) {
            ExternalPaymentId = apiPayment.Id;
            Status = apiPayment.Status;
            ErrorMessage = apiPayment.Error;
            Updated = Utils.Now();
        }

        public PaymentDto AsDto() => new() {
            PaymentId = PaymentId,
            IsReturn = IsReturn,
            IsTargettingBalance = IsTargettingBalance,
            IsBalanceRepayment = IsBalanceRepayment,
            Amount = Amount,
            Status = Status,
            ErrorMessage = ErrorMessage,
            Order = Order?.AsBasicDto(),
            Created = Created,
            Updated = Updated,
        };
    }
}
