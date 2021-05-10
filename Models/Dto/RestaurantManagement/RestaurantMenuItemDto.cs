#nullable enable
using System.Collections.Generic;
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
        public virtual MenuCategory MenuCategory { get; set; } = null!;

        public virtual ICollection<MenuItemOptionList> Options { get; set; } = null!;
        public MenuItem AsNewMenuItem() => new() {
            MenuCategoryId = MenuCategory.MenuCategoryId,
            Name = Name,
            Description = Description,
            Price = Price,
            Amount = Amount,
            AmountUnit = AmountUnit,
            ValidFrom = ValidFrom,
            Options = Options,
        };
    }
}
