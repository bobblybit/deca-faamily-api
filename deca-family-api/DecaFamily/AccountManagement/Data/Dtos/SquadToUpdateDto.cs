using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Dtos
{
    public class SquadToUpdateDto
    {
        [MaxLength(25, ErrorMessage = "Name cannot be more than 125")]
        public string Name { get; set; }

        [MaxLength(25, ErrorMessage = "Slugan cannot be more than 125")]
        public string Slugan { get; set; }
    }
}
