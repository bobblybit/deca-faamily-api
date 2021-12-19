using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Dtos
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
    }
}
