using System.Collections.Generic;
using NodaTime;

namespace pwr_msi.Models.Dto.RestaurantManagement {
    public class RestaurantMenuCategoryWithItemsDto : RestaurantMenuCategoryDto {
        public ICollection<RestaurantMenuItemDto> Items { get; set; }
    }
}
