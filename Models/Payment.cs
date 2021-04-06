namespace pwr_msi.Models {
    public class Payment {
        public int PaymentId { get; set; }
        public string ExternalPaymentId { get; set; }
        public bool IsReturn { get; set; }
        public bool IsFromBalance { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string ErrorMessage { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
