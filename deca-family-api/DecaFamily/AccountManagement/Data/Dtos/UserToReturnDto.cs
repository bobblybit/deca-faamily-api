using System.Collections.Generic;

namespace AccountManagement.Data.Dtos
{
    public class UserToReturnDto
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public StackToReturnDto MyStack { get; set; }
        public SquadToReturnDto MySquad { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public AddressDto Address { get; set; }
        public List<PhotoDto> UserPhotos { get; set; } = new List<PhotoDto>();
        public List<SocialHandleDto> SocialHandles { get; set; } = new List<SocialHandleDto>();
        public DepartmentDto Department { get; set; }
    }
}