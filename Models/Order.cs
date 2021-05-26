using System.Collections.Generic;
using System.Linq;
using NodaTime;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Enum;

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

        public Restaurant Restaurant { get; set; }
        public User Customer { get; set; }
        public User DeliveryPerson { get; set; }
        public Address Address { get; set; }

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
        
        public OrderDetailsDto AsDetailedDto(ICollection<OrderItem> items, ICollection<OrderItemCustomization> options) => new() {
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
            Items = items.Select(oi => oi.AsDto()).ToList(),
            Options = options,
            DeliveryPerson = DeliveryPerson.AsBasicDto()
        };
    }
}
