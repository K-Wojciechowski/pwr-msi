using System.Collections.Generic;

namespace pwr_msi.Models.Dto {
    public class RestaurantDetailDto {
        public string Name { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }

        public int AddressId { get; set; }
        public virtual Address Address { get; set; }

        public virtual ICollection<Cuisine> Cuisines { get; set; }
    }
}
