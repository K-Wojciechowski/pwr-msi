using System.Collections.Generic;
using NodaTime;
using pwr_msi.Models.Enum;

namespace pwr_msi.Models.Dto {
    public class OrderDto {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryNotes { get; set; }
        public OrderStatus Status { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }

        public RestaurantBasicDto Restaurant { get; set; } = null!;
        public UserBasicDto Customer { get; set; } = null!;
        public virtual Address Address { get; set; } = null!;

        public OrderTaskType LastTaskType => OrderTaskTypeSettings.taskTypeByStatus[Status];
        public ICollection<OrderItem> Items { get; set; }
        public ICollection<OrderItemCustomization> ItemOptions { get; set; }
    }
}
