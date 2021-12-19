using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Dtos
{
    public class PhotoToUpdateDto
    {
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public string PublicId { get; set; }
        [Required]
        public bool IsMain { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}
