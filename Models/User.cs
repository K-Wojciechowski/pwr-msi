using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using pwr_msi.Models.Dto.Auth;

namespace pwr_msi.Models {
    public class User {
        public int UserId { get; set; }

        [Required] public string Username { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required] public string Password { get; set; }

        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public decimal Balance { get; set; }

        [Required] public bool IsAdmin { get; set; }

        [Required] public bool IsVerified { get; set; }

        public int? BillingAddressId { get; set; }
        public virtual Address BillingAddress { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Restaurant> Restaurants { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<BalanceRepayment> BalanceRepayments { get; set; }
        public virtual List<RestaurantUser> RestaurantUsers { get; set; }

        public string FullName => FirstName + " " + LastName;

        public UserProfileDto AsProfile() {
            return new() {
                UserId = UserId,
                Username = Username,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Balance = Balance,
                BillingAddress = BillingAddress,
            };
        }
    }
}
