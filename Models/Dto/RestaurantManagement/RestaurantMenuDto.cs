using System.Collections.Generic;
using NodaTime;

namespace pwr_msi.Models.Dto {
    public class RestaurantMenuDto {
        public string Name { get; set; }
        public ZonedDateTime ValidFrom { get; set; }
        public ZonedDateTime? ValidUntil { get; set; }
        public virtual ICollection<MenuItem> Items { get; set; }
    }
}
