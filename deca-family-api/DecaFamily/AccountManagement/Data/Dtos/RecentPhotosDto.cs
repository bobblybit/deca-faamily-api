using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Dtos
{
    public class RecentPhotosDto
    {
        public string PhotoId { get; set; }
        public string AppUserId { get; set; }

        public string PhotoUrl { get; set; }
        public string PublicId { get; set; }
    }
}