using System.Collections.Generic;
using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class Restaurant {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<Cuisine> Cuisines { get; set; }

        public ICollection<MenuCategory> MenuCategories { get; set; }
        public ICollection<User> Users { get; set; }
        public List<RestaurantUser> RestaurantUsers { get; set; }

        public RestaurantBasicDto AsBasicDto() => new() {RestaruantId = RestaurantId, Name = Name};
    }
}
