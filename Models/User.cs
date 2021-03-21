using System.Collections;
using System.Collections.Generic;

namespace pwr_msi.Models {
    public class User {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int BillingAddressId { get; set; }
        public Address BillingAddress { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
        public List<RestaurantUser> RestaurantUsers { get; set; }
    }
}
