#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using pwr_msi.Models.Dto.RestaurantMenu;

namespace pwr_msi.Models.Dto {
    public class OrderItemDto {
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; } = null!;
        public RestaurantMenuItemDto MenuItem { get; set; } = null!;
        public ICollection<OrderItemCustomizationDto> Customizations { get; set; } = null!;

        public OrderItem AsNewOrderItem(decimal priceFromDb) {
            Debug.Assert(MenuItem.MenuItemId != null, "MenuItem.MenuItemId != null");
            return new OrderItem {
                Notes = Notes, Amount = Amount, TotalPrice = Amount * priceFromDb, MenuItemId = MenuItem.MenuItemId.Value, Customizations = Customizations.Select(c => c.AsNewCustomization()).ToList()
            };
        }
    }
}
