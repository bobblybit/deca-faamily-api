using System.Collections.Generic;

namespace AccountManagement.Data.Dtos
{
    public class DeleteUserPhotosDto
    {
        public IList<string> PublicIds { get; set; } 
    }
}
