using System.ComponentModel.DataAnnotations;


namespace AccountManagement.Data.Dtos
{
    public class SocialHandleToUpdateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Link { get; set; }
    }
}
