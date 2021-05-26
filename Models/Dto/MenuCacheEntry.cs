#nullable enable

using System.Collections.Generic;
using NodaTime;
using pwr_msi.Models.Dto.RestaurantMenu;

namespace pwr_msi.Models.Dto {
    public class MenuCacheEntry {
        public List<RestaurantMenuCategoryWithItemsDto> menuCategories { get; set; }
        public ZonedDateTime validAt { get; set; }
        public ZonedDateTime? expiresAt { get; set; }

        public MenuCacheEntry(List<RestaurantMenuCategoryWithItemsDto> menuCategories, ZonedDateTime validAt, ZonedDateTime? expiresAt) {
            this.menuCategories = menuCategories;
            this.validAt = validAt;
            this.expiresAt = expiresAt;
        }
    }
}
