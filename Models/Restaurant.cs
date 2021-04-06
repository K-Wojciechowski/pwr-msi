using System.Collections.Generic;
using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class Restaurant {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }

        public int AddressId { get; set; }
        public virtual Address Address { get; set; }

        public virtual ICollection<Cuisine> Cuisines { get; set; }
        public virtual ICollection<MenuCategory> MenuCategories { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual List<RestaurantUser> RestaurantUsers { get; set; }

        public RestaurantBasicDto AsBasicDto() => new() {RestaruantId = RestaurantId, Name = Name};
    }
}
