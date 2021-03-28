using System.ComponentModel.DataAnnotations;

namespace pwr_msi.Models.Dto.Auth {
    public class PasswordResetDto {
        [Required]
        [Compare(otherProperty: "RepeatPassword")]
        public string Password { get; set; }

        [Required]
        [Compare(otherProperty: "Password")]
        public string RepeatPassword { get; set; }
    }
}
