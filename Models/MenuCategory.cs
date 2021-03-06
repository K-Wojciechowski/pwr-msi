using System.Collections.Generic;
using System.Linq;
using NodaTime;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.RestaurantMenu;

namespace pwr_msi.Models {
    public class MenuCategory {
        public int MenuCategoryId { get; set; }
        public string Name { get; set; }
        public int MenuCategoryOrder { get; set; }
        public ZonedDateTime ValidFrom { get; set; }
        public ZonedDateTime? ValidUntil { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        public ICollection<MenuItem> Items { get; set; }
        public MenuCategoryWithItemsDto AsMenuDto() => new () {
            MenuCategoryId = MenuCategoryId,
            Name = Name,
            MenuCategoryOrder = MenuCategoryOrder,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            Items = Items.Select(mi => mi.AsDto()).ToList(),
        };
        public MenuCategoryDto AsCategoryDto() => new () {
            MenuCategoryId = MenuCategoryId,
            MenuCategoryOrder = MenuCategoryOrder,
            Name = Name,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
        };
        public void UpdateWithMenuCategoryDto(MenuCategoryDto mcDto) {
            ValidUntil = mcDto.ValidFrom ?? Utils.Now();
        }

        public void Invalidate(ZonedDateTime? validUntil = null) {
            ValidUntil = validUntil ?? Utils.Now();
        }
    }
}
