namespace AccountManagement.Data.Models
{
    public class Photo : BaseEntity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public string PhotoUrl { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }
    }
}