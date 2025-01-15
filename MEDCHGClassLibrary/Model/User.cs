using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Model
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? ImgUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Designation { get; set; }
        public string UserRole { get; set; } = null!;
        public int? ProviderId { get; set; }
        public Provider? Provider { get; }
        public string? Password { get; set; }
    }
}
