#nullable enable

using System.Collections.Generic;
using NodaTime;
using pwr_msi.Models.Dto.RestaurantMenu;

namespace pwr_msi.Models.Dto {
    public class MenuCacheEntry {
        public List<MenuCategoryWithItemsDto> menuCategories { get; set; }
        public ZonedDateTime validAt { get; set; }
        public ZonedDateTime? expiresAt { get; set; }

        public MenuCacheEntry(List<MenuCategoryWithItemsDto> menuCategories, ZonedDateTime validAt, ZonedDateTime? expiresAt) {
            this.menuCategories = menuCategories;
            this.validAt = validAt;
            this.expiresAt = expiresAt;
        }
    }
}
