#nullable enable
namespace pwr_msi.Models.Dto.PaymentGateway {
    public class PaymentCallbackDto {
       public string Id { get; set; } = null!;
       public string? External { get; set; }
       public PaymentStatus Status { get; set; }
       public string? Error { get; set; }
    }
}
