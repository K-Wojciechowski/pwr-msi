using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using NodaTime;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Enum;
using pwr_msi.Services;

namespace pwr_msi.Models {
    public class Order {
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int? DeliveryPersonId { get; set; }
        public int AddressId { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryNotes { get; set; }
        public OrderStatus Status { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }
        public ZonedDateTime? Delivered { get; set; }

        public virtual Restaurant Restaurant { get; set; }
        public virtual User Customer { get; set; }
        public virtual User DeliveryPerson { get; set; }
        public virtual Address Address { get; set; }

        public OrderTaskType LastTaskType => OrderTaskTypeSettings.taskTypeByStatus[Status];

        public OrderBasicDto AsBasicDto() => new() {
            OrderId = OrderId,
            Restaurant = Restaurant.AsBasicDto(),
            Customer = Customer.AsBasicDto(),
            Address = Address,
            Status = Status,
            TotalPrice = TotalPrice,
            DeliveryNotes = DeliveryNotes,
            Created = Created,
            Updated = Updated,
            Delivered = Delivered,
        };
        
        public OrderDto AsDto(ICollection<OrderItemCustomization> Options,ICollection<OrderItem> Items) => new() {
            OrderId = OrderId,
            Restaurant = Restaurant.AsBasicDto(),
            Customer = Customer.AsBasicDto(),
            Address = Address,
            Status = Status,
            TotalPrice = TotalPrice,
            DeliveryNotes = DeliveryNotes,
            Created = Created,
            Updated = Updated,
            ItemOptions = Options,
            Items = Items
        };
    }
}
