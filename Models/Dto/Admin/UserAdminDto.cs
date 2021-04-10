using System.ComponentModel.DataAnnotations;

namespace pwr_msi.Models.Dto.Admin {
    public class UserAdminDto {
        public int? UserId { get; set; }
        [Required] public string Username { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsVerified { get; set; }

        public Address BillingAddress { get; set; }

        public User AsNewUser() {
            return new() {
                Username = Username,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Balance = Balance,
                IsActive = IsActive,
                IsAdmin = IsAdmin,
                IsVerified = IsVerified,
                BillingAddress = BillingAddress,
            };
        }
    }

    public class UserAdminCreateDto : UserAdminDto {
        [Required]
        [Compare(otherProperty: "RepeatPassword")]
        public string Password { get; set; }

        [Required]
        [Compare(otherProperty: "Password")]
        public string RepeatPassword { get; set; }
    }
}
