using System.Collections.Generic;

namespace pwr_msi.Models.Dto {
    public class RestaurantDetailDto {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Logo {get; set; }

        public int AddressId { get; set; }
        public virtual Address Address { get; set; }

        public virtual ICollection<Cuisine> Cuisines { get; set; }
    }
}
