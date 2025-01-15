using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.DTO
{
    public class UserDTO
    {
        public int? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? UserRole { get; set; } 
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImgUrl { get; set; }
        public string? Designation { get; set; }
        public string? Device { get; set; }
        public string? Password { get; set; }
        public int? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? ProviderDisplayName { get; set; }
        public int? IsActive { get; set; }
        public int? CreatedAndUpdatedById { get; set; }
    }
}
