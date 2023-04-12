using BackEndProject.Entities;
using BackEndProject.Utilities.Enum;
using BackEndProject.ViewModel.Register_and_Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> _usermanager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AccountController(UserManager<User> usermanager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
		{
			_usermanager = usermanager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM account)
		{
			if (!ModelState.IsValid) return View();
			if (!account.Terms)
			{

				ModelState.AddModelError("Terms", "Please click Terms button");
				return View();
			}

			User user = new()
			{
				UserName = account.Username,
				Email = account.Email,
				FullName = string.Concat(account.Firstname, " ", account.Lastname)
			};
			IdentityResult result = await _usermanager.CreateAsync(user, account.Password);

			if (!result.Succeeded)
			{
				foreach (IdentityError message in result.Errors)
				{
					ModelState.AddModelError(" ", message.Description);
				}
				return View();
			}
			await _usermanager.AddToRoleAsync(user, Roles.User.ToString());
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginVM account)
		{
			if (!ModelState.IsValid) return View();

			User user = await _usermanager.FindByNameAsync(account.Username);
			if (user is null)
			{
				ModelState.AddModelError("", "Username or password is incorrect");
				return View();
			}
			Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, account.Password, account.RememberMe, true);

			if (!result.Succeeded)
			{
				if (result.IsLockedOut)
				{
					ModelState.AddModelError("", "Due to your efforts, our account was blocked for 5 minutes");
				}
				ModelState.AddModelError("", "Username or password is incorrect");
				return View();
			}
			return RedirectToAction("Index", "Home");
		}


		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		//public async Task CreateRoles()
		//{
		//	await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
		//	await _roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
		//	await _roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
		//}
	}

}
