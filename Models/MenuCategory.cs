using System.Collections.Generic;
using NodaTime;
using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class MenuCategory {
        public int MenuCategoryId { get; set; }
        public string Name { get; set; }
        public ZonedDateTime ValidFrom { get; set; }
        public ZonedDateTime? ValidUntil { get; set; }

        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<MenuItem> Items { get; set; }
        public RestaurantMenuDto AsManageMenuDto() => new () {
            Name = Name,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            Items = Items,
        };
        public RestaurantMenuCategoryDto AsManageCategoryDto() => new () {
            MenuCategoryId = MenuCategoryId,
            Name = Name,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
        };
        public void UpdateWithRestaurantMenuCategoryDto(RestaurantMenuCategoryDto mcDto) {
            Name = mcDto.Name;
            ValidUntil = mcDto.ValidFrom ?? new ZonedDateTime();
        }

        public void MakeMenuCategoryNonValid() {
            ValidUntil = new ZonedDateTime();
        }
    }
}
