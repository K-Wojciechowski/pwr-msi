using System.Collections.Generic;
using Microsoft.AspNetCore.Routing.Constraints;
using NodaTime;
using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class MenuItem {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public AmountUnit AmountUnit { get; set; }
        public ZonedDateTime ValidFrom { get; set; }
        public ZonedDateTime ValidUntil { get; set; }
        public int MenuOrder { get; set; }

        public int MenuCategoryId { get; set; }
        public virtual MenuCategory MenuCategory { get; set; }

        public virtual ICollection<MenuItemOptionList> Options { get; set; }
        public RestaurantMenuItemDto AsManageItemDto() => new () {
            Name = Name,
            Description = Description,
            Price = Price,
            Amount = Amount,
            AmountUnit = AmountUnit,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            MenuCategory = MenuCategory,
            Options = Options,
        };
        public void UpdateWithRestaurantMenuItemDto(RestaurantMenuItemDto mcDto) {
            Name = mcDto.Name;
            Description = mcDto.Description;
            Price = mcDto.Price;
            Amount = mcDto.Amount;
            AmountUnit = mcDto.AmountUnit;
            ValidUntil = mcDto.ValidFrom ?? new ZonedDateTime();
            Options = mcDto.Options;
        }
        public void MakeMenuItemNonValid() {
            ValidUntil = new ZonedDateTime();
        }
    }
}
