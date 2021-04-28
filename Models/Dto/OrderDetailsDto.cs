using System.Collections.Generic;
using NodaTime;
using pwr_msi.Models.Enum;

namespace pwr_msi.Models.Dto {
    public class OrderDetailsDto {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryNotes { get; set; }
        public OrderStatus Status { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }
        public ZonedDateTime? Delivered { get; set; }

        public virtual RestaurantBasicDto Restaurant { get; set; }
        public virtual UserBasicDto Customer { get; set; }
        public virtual UserBasicDto DeliveryPerson { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }
    }
}
