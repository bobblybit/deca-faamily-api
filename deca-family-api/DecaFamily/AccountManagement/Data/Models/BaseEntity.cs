using System;

namespace AccountManagement.Data.Models
{
    public class BaseEntity
    {
        public string Id { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow.ToString();
            UpdatedAt = DateTime.UtcNow.ToString();
        }
    }
}