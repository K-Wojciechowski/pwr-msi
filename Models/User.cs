﻿using System.Collections.Generic;
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
        public Address BillingAddress { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<BalanceRepayment> BalanceRepayments { get; set; }
        public List<RestaurantUser> RestaurantUsers { get; set; }

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
