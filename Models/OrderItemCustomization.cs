using pwr_msi.Models.Dto;

#nullable enable
namespace pwr_msi.Models {
    public class OrderItemCustomization {
        public int OrderItemCustomizationId { get; set; }
        public int OrderItemId { get; set; }
        public int MenuItemOptionItemId { get; set; }

        public OrderItem OrderItem { get; set; } = null!;
        public MenuItemOptionItem MenuItemOptionItem { get; set; } = null!;

        public OrderItemCustomization(int orderItemCustomizationId, int menuItemOptionItemId) {
            OrderItemCustomizationId = orderItemCustomizationId;
            MenuItemOptionItemId = menuItemOptionItemId;
        }

        public OrderItemCustomizationDto AsDto() => new(OrderItemCustomizationId, MenuItemOptionItem.AsDto());
    }
}
