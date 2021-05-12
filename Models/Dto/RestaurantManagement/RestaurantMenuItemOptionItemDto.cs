namespace pwr_msi.Models.Dto.RestaurantManagement {
    public class RestaurantMenuItemOptionItemDto {
        public int MenuItemOptionItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MenuItemOptionItemOrder { get; set; }
        public int MenuItemOptionListId { get; set; }

        public MenuItemOptionItem AsNewMenuItemOptionItem(int listId) => new() {
                MenuItemOptionItemId = MenuItemOptionItemId,
                Name = Name,
                Price = Price,
                MenuItemOptionListId = listId,
        };
    }
}
