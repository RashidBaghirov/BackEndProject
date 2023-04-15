using Microsoft.AspNetCore.Identity;

namespace BackEndProject.Entities
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public bool? IsAdmin { get; set; } = false;
    }
}
