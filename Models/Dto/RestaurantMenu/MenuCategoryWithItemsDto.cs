using System.Collections.Generic;

namespace pwr_msi.Models.Dto.RestaurantMenu {
    public class MenuCategoryWithItemsDto : MenuCategoryDto {
        public ICollection<MenuItemDto> Items { get; set; }
    }
}
