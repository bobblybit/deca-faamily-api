using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Models
{
    public class Department : BaseEntity
    {

        [MaxLength(125, ErrorMessage = "Street cannot be more than 125")]
        public string Name { get; set; }

        [MaxLength(125, ErrorMessage = "Street cannot be more than 125")]
        public string Position { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public string CompanyId { get; set; }
        public Company Company { get; set; }

    }
}