using System.ComponentModel.DataAnnotations;

namespace pwr_msi.Models.Dto.Auth {
    public class ForgotPasswordDto {
        [EmailAddress] [Required] public string Email { get; set; }
    }
}
