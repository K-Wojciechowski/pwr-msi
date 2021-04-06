using System.ComponentModel.DataAnnotations;

namespace pwr_msi.Models.Dto.Auth {
    public class EditProfileDto {
        [Required] [EmailAddress] public string Email { get; set; }

        [Compare(otherProperty: "RepeatPassword")]
        public string Password { get; set; }

        [Compare(otherProperty: "Password")] public string RepeatPassword { get; set; }

        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public Address BillingAddress { get; set; }
    }
}
