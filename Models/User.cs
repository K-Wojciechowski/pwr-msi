using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.Admin;
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

        [Required] public bool IsActive { get; set; }

        [Required] public bool IsAdmin { get; set; }

        [Required] public bool IsVerified { get; set; }

        public int? BillingAddressId { get; set; }
        public Address BillingAddress { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public List<RestaurantUser> RestaurantUsers { get; set; }

        public string FullName => FirstName + " " + LastName;

        public UserProfileDto AsProfile() => new() {
            UserId = UserId,
            Username = Username,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            Balance = Balance,
            BillingAddress = BillingAddress,
        };

        public UserAdminDto AsAdminDto() => new() {
            UserId = UserId,
            Username = Username,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            Balance = Balance,
            BillingAddress = BillingAddress,
            IsActive = IsActive,
            IsAdmin = IsAdmin,
            IsVerified = IsVerified,
        };

        public UserBasicDto AsBasicDto() => new() {
            UserId = UserId, Username = Username, FirstName = FirstName, LastName = LastName,
        };

        public void UpdateWithAdminDto(UserAdminDto userAdminDto) {
            Username = userAdminDto.Username;
            Email = userAdminDto.Email;
            FirstName = userAdminDto.FirstName;
            LastName = userAdminDto.LastName;
            Balance = userAdminDto.Balance;
            IsActive = userAdminDto.IsActive;
            IsAdmin = userAdminDto.IsAdmin;
            IsVerified = userAdminDto.IsVerified;

            if (BillingAddress != null && userAdminDto.BillingAddress != null) {
                BillingAddress.Update(userAdminDto.BillingAddress);
            } else {
                BillingAddress = userAdminDto.BillingAddress;
            }
        }
        public void UpdateWithProfileDto(UserProfileDto dto) {
            Username = dto.Username;
            FirstName = dto.FirstName;
            LastName = dto.LastName;
        }
    }
}
