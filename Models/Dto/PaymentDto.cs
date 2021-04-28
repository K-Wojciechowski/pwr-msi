#nullable enable
namespace pwr_msi.Models.Dto {
    public class PaymentDto {
        public int PaymentId { get; set; }
        public bool IsReturn { get; set; }
        public bool IsFromBalance { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public OrderBasicDto Order { get; set; } = null!;
    }
}
