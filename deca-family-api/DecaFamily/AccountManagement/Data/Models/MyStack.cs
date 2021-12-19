using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Models
{
    public class MyStack : BaseEntity
    {
        [MaxLength(25, ErrorMessage = "Name cannot be more than 25")]
        public string Name { get; set; }

        [MaxLength(25, ErrorMessage = "Slugan cannot be more than 25")]
        public string Slugan { get; set; }

        public ICollection<AppUser> AppUsers { get; set; }

        public MyStack()
        {
            AppUsers = new HashSet<AppUser>();
        }
    }
}