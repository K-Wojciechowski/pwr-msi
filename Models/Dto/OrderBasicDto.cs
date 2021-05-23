#nullable enable
using NodaTime;
using pwr_msi.Models.Enum;

namespace pwr_msi.Models.Dto {
    public class OrderBasicDto {
        public int OrderId { get; set; }
        public RestaurantBasicDto Restaurant { get; set; } = null!;
        public UserBasicDto Customer { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string? DeliveryNotes { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }
        public ZonedDateTime? Delivered { get; set; }
    }
}
