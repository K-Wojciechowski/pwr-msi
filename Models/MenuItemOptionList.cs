using System.Collections.Generic;

namespace pwr_msi.Models {
    public class MenuItemOptionList {
        public int MenuItemOptionListId { get; set; }
        public string Name { get; set; }
        public bool IsMultipleChoice { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public ICollection<MenuItemOptionItem> Items { get; set; }
    }
}
