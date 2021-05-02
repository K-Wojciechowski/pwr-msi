namespace pwr_msi.Models.Dto.PaymentGateway {
    public class PaymentInfoDto {
        public string Id { get; set; }
        public string Url { get; set; }
        public PaymentApiDto Payment { get; set; }
    }
}
