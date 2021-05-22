#nullable enable
using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace pwr_msi.Models.Dto.RestaurantManagement {
    public class RestaurantMenuItemDto {
        public int? MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public AmountUnit AmountUnit { get; set; }
        public ZonedDateTime ValidFrom { get; set; }
        public ZonedDateTime? ValidUntil { get; set; }
        public int MenuCategoryId { get; set; }
        public int MenuOrder { get; set; }
        public ICollection<RestaurantMenuItemOptionListDto> Options { get; set; } = null!;

        public MenuItem AsNewMenuItem() => new() {
            MenuCategoryId = MenuCategoryId,
            Name = Name,
            Description = Description,
            Image = Image,
            Price = Price,
            Amount = Amount,
            AmountUnit = AmountUnit,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            MenuOrder = MenuOrder,
            Options = Options.Select(mo => mo.AsNewMenuItemOptionList()).ToList(),
        };
    }
}
