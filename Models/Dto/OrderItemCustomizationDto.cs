#nullable enable

using pwr_msi.Models.Dto.RestaurantMenu;

namespace pwr_msi.Models.Dto {
    public class OrderItemCustomizationDto {
        public int OrderItemCustomizationId { get; }
        public MenuItemOptionItemDto MenuItemOptionItem { get; }

        public OrderItemCustomizationDto(int orderItemCustomizationId,
            MenuItemOptionItemDto menuItemOptionItem) {
            OrderItemCustomizationId = orderItemCustomizationId;
            MenuItemOptionItem = menuItemOptionItem;
        }

        public OrderItemCustomization AsNewCustomization() =>
            new(OrderItemCustomizationId, MenuItemOptionItem.MenuItemOptionListId);
    }
}
