using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Models
{
    public class Company : BaseEntity
    {
        [MaxLength(125, ErrorMessage = "Name cannot be more than 125")]
        public string Name { get; set; }

        public string Logo { get; set; }

        [MaxLength(25, ErrorMessage = "Motto cannot be more than 25")]
        public string Motto { get; set; }

        [MaxLength(250, ErrorMessage = "Mission cannot be more than 250")]
        public string Mission { get; set; }

        [MaxLength(250, ErrorMessage = "Vision cannot be more than 250")]
        public string Vision { get; set; }


        [MaxLength(150, ErrorMessage = "Description cannot be more than 150")]
        public string Description { get; set; }

        public ICollection<Department> Departments { get; set; }
        public ICollection<CompanyAddress> CompanyAddresses { get; set; }
        public ICollection<CompanyWebsites> CompanyWebsites { get; set; }

        public Company()
        {
            Departments = new HashSet<Department>();
            CompanyAddresses = new HashSet<CompanyAddress>();
            CompanyWebsites = new HashSet<CompanyWebsites>();
        }

    }
}