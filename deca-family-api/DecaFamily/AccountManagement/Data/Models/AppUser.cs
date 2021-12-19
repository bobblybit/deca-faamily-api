using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Models
{
    public class AppUser : IdentityUser
    {
        [MaxLength(250, ErrorMessage = "First Name can not be longer than 250 characters")]
        public string FirstName { get; set; }

        [MaxLength(250, ErrorMessage = "Last Name can not be longer than 250 characters")]
        public string LastName { get; set; }

        public byte Gender { get; set; }
        public bool IsActive { get; set; }

        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString();
        public string UpdatedAt { get; set; } = DateTime.UtcNow.ToString();
        public AppUserAddress Address { get; set; }
        public Department Department { get; set; }


        public string MySquadId { get; set; }
        public MySquad MySquad { get; set; }
        public string MyStackId { get; set; }
        public MyStack MyStack { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<SocialHandles> SocialHandles { get; set; }

        public AppUser()
        {
            Photos = new HashSet<Photo>();
            SocialHandles = new HashSet<SocialHandles>();
        }

    }
}