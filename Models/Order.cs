#nullable enable
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
        public string DeliveryNotes { get; set; } = null!;
        public OrderStatus Status { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }
        public ZonedDateTime? Delivered { get; set; }

        public Restaurant Restaurant { get; set; } = null!;
        public User Customer { get; set; } = null!;
        public User? DeliveryPerson { get; set; }
        public Address Address { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = null!;

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

        public OrderDto AsDto() => new() {
            OrderId = OrderId,
            Restaurant = Restaurant.AsBasicDto(),
            Customer = Customer.AsBasicDto(),
            DeliveryPerson = DeliveryPerson?.AsBasicDto(),
            Address = Address,
            Status = Status,
            TotalPrice = TotalPrice,
            DeliveryNotes = DeliveryNotes,
            Created = Created,
            Updated = Updated,
            Delivered = Delivered,
            Items = Items.Select(item => item.AsDto()).ToList()
        };
    }
}
