using System.Collections.Generic;
using Newtonsoft.Json;

namespace pwr_msi.Models {
    public class Cuisine {
        public int CuisineId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
