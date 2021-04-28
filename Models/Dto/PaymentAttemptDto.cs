#nullable enable
namespace pwr_msi.Models.Dto {
    public class PaymentAttemptDto {
        public int PaymentId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? PaymentUrl { get; set; } = null;
    }
}
