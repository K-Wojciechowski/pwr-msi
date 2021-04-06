using System.ComponentModel.DataAnnotations;

namespace pwr_msi.Models.Dto.Auth {
    public class NewUserDto {
        [Required] public string Username { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [Compare(otherProperty: "RepeatPassword")]
        public string Password { get; set; }

        [Required]
        [Compare(otherProperty: "Password")]
        public string RepeatPassword { get; set; }

        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        public Address BillingAddress { get; set; }
    }
}
