using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
