using Microsoft.AspNetCore.Identity;

namespace BackEndProject.Entities
{
	public class User : IdentityUser
	{
		public string? FullName { get; set; }
		public List<Comment>? Comments { get; set; }
		public User()
		{
			Comments = new();
		}
	}
}
