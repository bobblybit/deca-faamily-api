using System.ComponentModel.DataAnnotations;


namespace ArticleService.Data.Dtos
{
    public class CategoryToAddDto
    {
        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
