using System.Collections.Generic;
using NodaTime;

namespace pwr_msi.Models {
    public class MenuCategory {
        public int MenuCategoryId { get; set; }
        public string Name { get; set; }
        public ZonedDateTime ValidFrom { get; set; }
        public ZonedDateTime ValidUntil { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        public ICollection<MenuItem> Items { get; set; }
    }
}
