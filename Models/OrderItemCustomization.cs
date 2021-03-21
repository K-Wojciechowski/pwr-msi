namespace pwr_msi.Models {
    public class OrderItemCustomization {
        public int OrderItemCustomizationId { get; set; }
        public int OrderItemId { get; set; }
        public int MenuItemOptionItemId { get; set; }

        public OrderItem OrderItem { get; set; }
        public MenuItemOptionItem MenuItemOptionItem { get; set; }
    }
}
