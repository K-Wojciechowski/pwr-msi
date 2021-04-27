#nullable enable
namespace pwr_msi.Models.Dto.PaymentGateway {
    public class PaymentRequestDto {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string Payer { get; set; } = null!;
        public string Payee { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ExternalId { get; set; }
        public bool IsReturn { get; set; }
    }
}
