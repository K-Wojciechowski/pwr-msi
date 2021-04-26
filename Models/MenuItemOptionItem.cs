using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class MenuItemOptionItem {
        public int MenuItemOptionItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MenuItemOptionListId { get; set; }
        public virtual MenuItemOptionList MenuItemOptionList { get; set; }
        
        public RestaurantMenuItemOptionItemDto AsManageOptionItemDto() => new () {
            MenuItemOptionItemId = MenuItemOptionItemId,
            Name = Name,
            Price = Price,
            MenuItemOptionListId = MenuItemOptionListId
        };
        public void UpdateWithRestaurantMenuItemOptionItemDto(RestaurantMenuItemOptionItemDto mioiDto) {
            Name = mioiDto.Name;
            Price = mioiDto.Price;
        }
    }
    
}
