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
        public decimal Balance { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsVerified { get; set; }

        public int BillingAddressId { get; set; }
        public Address BillingAddress { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<BalanceRepayment> BalanceRepayments { get; set; }
        public List<RestaurantUser> RestaurantUsers { get; set; }

        public string FullName => FirstName + " " + LastName;
    }
}
