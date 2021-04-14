using System.Collections.Generic;
using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class MenuItemOptionList {
        public int MenuItemOptionListId { get; set; }
        public string Name { get; set; }
        public bool IsMultipleChoice { get; set; }
        public int MenuItemId { get; set; }
        public virtual MenuItem MenuItem { get; set; }

        public virtual ICollection<MenuItemOptionItem> Items { get; set; }
        
        public RestaurantMenuItemOptionListDto AsManageOptionListDto() => new () {
            MenuItemOptionListId = MenuItemOptionListId,
            Name = Name,
            IsMultipleChoice= IsMultipleChoice,
            MenuItemId = MenuItemId
        };
    }
}
