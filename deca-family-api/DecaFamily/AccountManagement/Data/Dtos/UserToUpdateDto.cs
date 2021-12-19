using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Dtos
{
    public class UserToUpdateDto
    {
        [Required]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "First Name can not be lessthan 3 and longer than 250 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Last Name can not be lessthan 3 and longer than 250 characters")]
        public string LastName { get; set; }

        [Required]
        public byte Gender { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 characters")]
        public string PhoneNumber { get; set; }

        public string MyStackId { get; set; }
        public string MySquadId { get; set; }

        public AddressDto Address { get; set; }
        public DepartmentToUpdateDto Department { get; set; }

        public IEnumerable<SocialHandleToUpdateDto> SocialHandles { get; set; }

       
    }
}
