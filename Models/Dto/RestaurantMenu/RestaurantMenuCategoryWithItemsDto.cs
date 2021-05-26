using System.Collections.Generic;

namespace pwr_msi.Models.Dto.RestaurantMenu {
    public class RestaurantMenuCategoryWithItemsDto : RestaurantMenuCategoryDto {
        public ICollection<RestaurantMenuItemDto> Items { get; set; }
    }
}
