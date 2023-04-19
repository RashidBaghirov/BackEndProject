using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Services;
using BackEndProject.Utilities.Enum;
using BackEndProject.ViewModel.Register_and_Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> _usermanager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ProductDbContext _context;

		public AccountController(UserManager<User> usermanager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ProductDbContext _context)
		{
			_usermanager = usermanager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_context = _context;
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

			//// Send welcome email to the user
			//string subject = "Welcome to our website!";
			//string body = $"Dear {user.FullName},<br><br>Welcome to our website! We are glad you joined us.";
			//_emailService.Send(user.Email, subject, body);

			await _usermanager.AddToRoleAsync(user, Roles.User.ToString());
			return RedirectToAction(nameof(Login));
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
			return RedirectToAction(nameof(Login));
		}




		//public async Task CreateRoles()
		//{
		//	await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
		//	await _roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
		//	await _roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
		//}



		public async Task<IActionResult> Details()
		{
			User user = await _usermanager.FindByNameAsync(User.Identity.Name);
			if (user is null)
			{
				return RedirectToAction(nameof(Login));
			}
			ProfileVM profileVM = new()
			{
				Email = user.Email,
				UserName = user.UserName,
				FullName = user.FullName
			};

			return View(profileVM);
		}

		[HttpPost]
		public async Task<IActionResult> Details(ProfileVM profileVM)
		{
			if (!ModelState.IsValid) return View();

			User member = await _usermanager.FindByNameAsync(User.Identity.Name);

			if (!string.IsNullOrWhiteSpace(profileVM.ConfirmNewPassword) && !string.IsNullOrWhiteSpace(profileVM.NewPassword))
			{
				var passwordChangeResult = await _usermanager.ChangePasswordAsync(member, profileVM.CurrentPassword, profileVM.NewPassword);

				if (!passwordChangeResult.Succeeded)
				{
					foreach (var item in passwordChangeResult.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}

					return View();
				}

			}

			if (member.Email != profileVM.Email && _usermanager.Users.Any(x => x.NormalizedEmail == profileVM.Email.ToUpper()))
			{
				ModelState.AddModelError("Email", "This email has already been taken!");
				return View();
			}
			member.Email = profileVM.Email;
			member.UserName = profileVM.UserName;


			var result = await _usermanager.UpdateAsync(member);

			if (!result.Succeeded)
			{
				foreach (var item in result.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}

				return View();
			}
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(Login));
		}


		public async Task<IActionResult> MyOrder()
		{
			//User user = await _usermanager.FindByNameAsync(User.Identity.Name);
			//if (user == null)
			//{
			//	return RedirectToAction(nameof(Login));
			//}
			//List<Order> orders = _context.Orders
			//	.Include(x => x.OrderItems)
			//	.Where(x => x.UserId == user.Id)
			//	.ToList();

			return View();
		}

	}

}
