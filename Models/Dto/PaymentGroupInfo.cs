#nullable enable
namespace pwr_msi.Models.Dto {
    public class PaymentGroupInfo {
        public decimal PaidFromBalance { get; set; }
        public decimal PaidExternally { get; set; }
        public string? PaymentUrl { get; set; }
        public int? ExternalPaymentId { get; set; }
        public bool IsPaid { get; set; }
    }
}
