using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.DTO
{
    public class LoginDTO
    {
        public string? UserName { get; set; }
        public string? ProviderName { get; set; }
        public string? Password { get; set; }
    }
}