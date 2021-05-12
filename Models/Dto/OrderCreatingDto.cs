using System.Collections.Generic;
using NodaTime;
using pwr_msi.Models.Enum;

namespace pwr_msi.Models.Dto {
    public class OrderCreatingDto {
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public string DeliveryNotes { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public ICollection<MenuItemOptionItem> OptionItems { get; set; }
    }
}
