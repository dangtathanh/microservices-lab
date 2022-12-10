using Microsoft.AspNetCore.Identity;

namespace GRPCLab.IdentityService.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string DisplayName { get; set; }
    }
}
