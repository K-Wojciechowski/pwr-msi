using NodaTime;

#nullable enable
namespace pwr_msi.Models.Dto {
    public class PaymentDto {
        public int PaymentId { get; set; }
        public bool IsReturn { get; set; }
        public bool IsTargettingBalance { get; set; }
        public bool IsBalanceRepayment { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public OrderBasicDto? Order { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }
    }
}
