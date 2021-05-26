using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.RestaurantMenu;

namespace pwr_msi.Models {
    public class MenuItemOptionItem {
        public int MenuItemOptionItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MenuItemOptionItemOrder { get; set; }
        public int MenuItemOptionListId { get; set; }
        public MenuItemOptionList MenuItemOptionList { get; set; }
        
        public MenuItemOptionItemDto AsDto() => new () {
            MenuItemOptionItemId = MenuItemOptionItemId,
            Name = Name,
            Price = Price,
            MenuItemOptionItemOrder = MenuItemOptionItemOrder,
            MenuItemOptionListId = MenuItemOptionListId
        };
        public void UpdateWithRestaurantMenuItemOptionItemDto(MenuItemOptionItemDto mioiDto) {
            Name = mioiDto.Name;
            Price = mioiDto.Price;
        }

        public MenuItemOptionItem CreateNewWithList(int optionListId) => new() {
            Name = Name, Price = Price, MenuItemOptionListId = optionListId,
        };
    }
    
}
