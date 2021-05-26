namespace pwr_msi.Models.Dto.RestaurantMenu {
    public class RestaurantMenuItemOptionItemDto {
        public int MenuItemOptionItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MenuItemOptionItemOrder { get; set; }
        public int MenuItemOptionListId { get; set; }

        public MenuItemOptionItem AsNewMenuItemOptionItem() => new() {
                Name = Name,
                Price = Price,
                MenuItemOptionItemOrder = MenuItemOptionItemOrder,
        };
    }
}
