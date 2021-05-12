#nullable enable
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.RestaurantManagement;

namespace pwr_msi.Models {
    public class MenuItem {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public AmountUnit AmountUnit { get; set; }
        public ZonedDateTime ValidFrom { get; set; }
        public ZonedDateTime? ValidUntil { get; set; }
        public int MenuOrder { get; set; }

        public int MenuCategoryId { get; set; }
        public virtual MenuCategory MenuCategory { get; set; } = null!;

        public virtual ICollection<MenuItemOptionList> Options { get; set; } = null!;

        public RestaurantMenuItemDto AsManageItemDto() => new() {
            MenuItemId = MenuItemId,
            Name = Name,
            Description = Description,
            Image = Image,
            Price = Price,
            Amount = Amount,
            AmountUnit = AmountUnit,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            MenuCategoryId = MenuCategoryId,
            Options = Options.Select(ol => ol.AsManageOptionListDto()).ToList(),
        };

        public void UpdateWithRestaurantMenuItemDto(RestaurantMenuItemDto mcDto) {
            ValidUntil = mcDto.ValidFrom;
        }

        public void Invalidate(ZonedDateTime? validUntil = null) {
            ValidUntil = validUntil ?? new ZonedDateTime();
        }


        public MenuItem CreateNewWithCategory(int menuCategoryId, ZonedDateTime validFrom) => new() {
            Name = Name,
            Description = Description,
            Image = Image,
            Price = Price,
            Amount = Amount,
            AmountUnit = AmountUnit,
            ValidFrom = validFrom,
            ValidUntil = ValidUntil,
            MenuCategoryId = menuCategoryId,
        };
    }
}
