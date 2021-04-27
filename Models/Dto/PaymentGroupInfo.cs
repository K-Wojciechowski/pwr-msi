#nullable enable
namespace pwr_msi.Models.Dto {
    public class PaymentGroupInfo {
        public decimal PaidFromBalance { get; set; }
        public decimal PaidExternally { get; set; }
        public string? PaymentLink { get; set; }
        public bool IsPaid { get; set; }
    }
}
