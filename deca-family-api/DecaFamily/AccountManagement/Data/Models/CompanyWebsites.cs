namespace AccountManagement.Data.Models
{
    public class CompanyWebsites : BaseEntity
    {
        public string CompanyId { get; set; }
        public Company Company { get; set; }

        public string Logo { get; set; }
        public string WebsiteUrl { get; set; }
    }
}