using System.Collections.Generic;

namespace AccountManagement.Data.Dtos
{
    public class UsersToReturnDto
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MyStack { get; set; }
        public string MySquad { get; set; }
        public PhotoDto MainPhoto { get; set; }
        public List<SocialHandleDto> SocialHandles { get; set; } = new List<SocialHandleDto>();
    }
}