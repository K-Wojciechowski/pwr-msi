namespace pwr_msi.Models {
    public class BalanceRepayment {
        public int BalanceRepaymentId { get; set; }
        public string ExternalRepaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string ErrorMessage { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
