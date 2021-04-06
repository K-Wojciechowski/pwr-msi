using System.Collections.Generic;

namespace pwr_msi.Models {
    public class Cuisine {
        public int CuisineId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
