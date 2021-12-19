using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Dtos
{
    public class StackToAddDto
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Name cannot be more than 25")]
        public string Name { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Slugan cannot be more than 25")]
        public string Slugan { get; set; }

    }
}
