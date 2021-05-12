﻿using System.Collections.Generic;
using System.Linq;
using NodaTime;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.RestaurantManagement;

namespace pwr_msi.Models {
    public class MenuCategory {
        public int MenuCategoryId { get; set; }
        public string Name { get; set; }
        public int MenuCategoryOrder { get; set; }
        public ZonedDateTime ValidFrom { get; set; }
        public ZonedDateTime? ValidUntil { get; set; }

        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<MenuItem> Items { get; set; }
        public RestaurantMenuCategoryWithItemsDto AsManageMenuDto() => new () {
            MenuCategoryId = MenuCategoryId,
            Name = Name,
            MenuCategoryOrder = MenuCategoryOrder,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            Items = Items.Select(mi => mi.AsManageItemDto()).ToList(),
        };
        public RestaurantMenuCategoryDto AsManageCategoryDto() => new () {
            MenuCategoryId = MenuCategoryId,
            MenuCategoryOrder = MenuCategoryOrder,
            Name = Name,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
        };
        public void UpdateWithRestaurantMenuCategoryDto(RestaurantMenuCategoryDto mcDto) {
            ValidUntil = mcDto.ValidFrom ?? new ZonedDateTime();
        }

        public void Invalidate(ZonedDateTime? validUntil = null) {
            ValidUntil = validUntil ?? new ZonedDateTime();
        }
    }
}
