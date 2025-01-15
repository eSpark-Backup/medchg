using Microsoft.AspNetCore.Identity;

namespace MEDCHGClassLibrary.DbContext
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProviderName { get; set; }
    }

}
