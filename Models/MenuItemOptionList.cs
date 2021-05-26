using System.Collections.Generic;
using System.Linq;
using pwr_msi.Models.Dto.RestaurantManagement;

namespace pwr_msi.Models {
    public class MenuItemOptionList {
        public int MenuItemOptionListId { get; set; }
        public string Name { get; set; }
        public bool IsMultipleChoice { get; set; }
        public int MenuItemOptionListOrder { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public ICollection<MenuItemOptionItem> Items { get; set; }

        public RestaurantMenuItemOptionListDto AsManageOptionListDto() => new() {
            MenuItemOptionListId = MenuItemOptionListId,
            Name = Name,
            IsMultipleChoice = IsMultipleChoice,
            MenuItemOptionListOrder = MenuItemOptionListOrder,
            MenuItemId = MenuItemId,
            Items = Items.Select(oi => oi.AsManageOptionItemDto()).ToList()
        };

        public MenuItemOptionList CreateNewWithItem(int menuItemId) => new() {
            Name = Name, IsMultipleChoice = IsMultipleChoice, MenuItemId = menuItemId,
        };
    }
}
