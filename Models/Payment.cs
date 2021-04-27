#nullable enable
using pwr_msi.Models.Dto.PaymentGateway;

namespace pwr_msi.Models {
    public class Payment {
        public int PaymentId { get; set; }
        public string? ExternalPaymentId { get; set; }
        public bool IsReturn { get; set; }
        public bool IsFromBalance { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? ErrorMessage { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public bool CanPay => Status == PaymentStatus.CREATED || Status == PaymentStatus.REQUESTED;

        public PaymentRequestDto AsRequestDto() => new() {
            Amount = Amount,
            Currency = Constants.DefaultCurrency,
            Payer = Order.Customer.FullName,
            Payee = Order.Restaurant.Name,
            Description = OrderId.ToString(),
        };

        public void UpdateFromApi(PaymentApiDto apiPayment) {
            ExternalPaymentId = apiPayment.Id;
            Status = apiPayment.Status;
            ErrorMessage = apiPayment.Error;
        }
    }
}
