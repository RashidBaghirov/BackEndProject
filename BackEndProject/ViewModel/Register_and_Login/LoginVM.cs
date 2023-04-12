using System.ComponentModel.DataAnnotations;

namespace BackEndProject.ViewModel.Register_and_Login
{
	public class LoginVM
	{

		public string Username { get; set; }

		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}
