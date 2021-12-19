using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Dtos
{
    public class PhotoToAddDto
    {
        [Required]
        public IFormFile Photo { get; set; }
        [Required]
        public bool IsMain { get; set; }
    }
}
