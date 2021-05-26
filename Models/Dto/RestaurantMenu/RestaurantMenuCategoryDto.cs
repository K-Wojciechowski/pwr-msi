using NodaTime;

namespace pwr_msi.Models.Dto.RestaurantMenu {
    public class RestaurantMenuCategoryDto {
        public int MenuCategoryId { get; set; }
        public int MenuCategoryOrder { get; set; }
        public string Name { get; set; }
        public ZonedDateTime? ValidFrom { get; set; }
        public ZonedDateTime? ValidUntil { get; set; }
        
        public MenuCategory AsNewMenuCategory(int restaurantId) => new() {
            RestaurantId = restaurantId,
            Name = Name,
            MenuCategoryOrder = MenuCategoryOrder,
            ValidFrom = ValidFrom ?? Utils.Now(),
        };
    }
}
