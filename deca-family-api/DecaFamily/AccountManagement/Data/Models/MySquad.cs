using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Models
{
    public class MySquad : BaseEntity
    {
        [MaxLength(25, ErrorMessage = "Name cannot be more than 125")]
        public string Name { get; set; }

        [MaxLength(25, ErrorMessage = "Slugan cannot be more than 125")]
        public string Slugan { get; set; }

        public ICollection<AppUser> AppUsers { get; set; }
        public MySquad()
        {
            AppUsers = new HashSet<AppUser>();
        }
    }
}