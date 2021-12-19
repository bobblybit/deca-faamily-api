using System;

namespace AccountManagement.Data.Models
{
    public class SocialHandles
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Link { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
