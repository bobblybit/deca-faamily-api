using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Dtos
{
    public class StackToUpdateDto
    {
        [MaxLength(25, ErrorMessage = "Name cannot be more than 25")]
        public string Name { get; set; }

        [MaxLength(25, ErrorMessage = "Slugan cannot be more than 25")]
        public string Slugan { get; set; }
    }
}
