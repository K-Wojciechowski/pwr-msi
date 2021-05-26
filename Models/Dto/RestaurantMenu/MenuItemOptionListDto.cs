using System.Collections.Generic;
using System.Linq;

namespace pwr_msi.Models.Dto.RestaurantMenu {
    public class MenuItemOptionListDto {
        public int MenuItemOptionListId { get; set; }
        public string Name { get; set; }
        public bool IsMultipleChoice { get; set; }
        public int MenuItemOptionListOrder { get; set; }
        public int MenuItemId { get; set; }
        public ICollection<MenuItemOptionItemDto> Items { get; set; }

        public MenuItemOptionList AsNewMenuItemOptionList() => new() {
            Name = Name,
            IsMultipleChoice = IsMultipleChoice,
            MenuItemOptionListOrder = MenuItemOptionListOrder,
            Items = Items.Select(mi => mi.AsNewMenuItemOptionItem()).ToList()
        };
    }
}
