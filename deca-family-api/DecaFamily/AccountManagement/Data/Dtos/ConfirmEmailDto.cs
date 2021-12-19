using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Dtos
{
    public class ConfirmEmailDto
    {

        [Required]
        [EmailAddress(ErrorMessage = "This email is invalid")]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
