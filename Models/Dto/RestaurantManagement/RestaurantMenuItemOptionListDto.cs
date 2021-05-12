using System.Collections.Generic;
using System.Linq;

namespace pwr_msi.Models.Dto.RestaurantManagement {
    public class RestaurantMenuItemOptionListDto {
        public int MenuItemOptionListId { get; set; }
        public string Name { get; set; }
        public bool IsMultipleChoice { get; set; }
        public int MenuItemOptionListOrder { get; set; }
        public int MenuItemId { get; set; }
        public ICollection<RestaurantMenuItemOptionItemDto> Items { get; set; }

        public MenuItemOptionList AsNewMenuItemOptionList(int itemId) => new() {
            MenuItemOptionListId = MenuItemOptionListId,
            Name = Name,
            IsMultipleChoice = IsMultipleChoice,
            MenuItemOptionListOrder = MenuItemOptionListOrder,
            MenuItemId = itemId,
            Items = Items.Select(mi => mi.AsNewMenuItemOptionItem(MenuItemOptionListId)).ToList()
        };
    }
}
