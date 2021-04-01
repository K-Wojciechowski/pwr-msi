using System.Collections.Generic;
using NodaTime;

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
    }
}
