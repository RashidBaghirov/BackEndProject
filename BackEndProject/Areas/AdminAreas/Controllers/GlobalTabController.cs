using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
	[Area("AdminAreas")]
	[Authorize(Roles = "Admin,Moderator")]
	public class GlobalTabController : Controller
	{

		private readonly ProductDbContext _context;
		public GlobalTabController(ProductDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			IEnumerable<GlobalTab> globalTabs = _context.GlobalTabs.AsEnumerable();
			return View(globalTabs);
		}


		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(GlobalTab NewGlobalTab)
		{

			if (!ModelState.IsValid)
			{
				foreach (string message in ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
				{
					ModelState.AddModelError("", message);
				}
				return View();
			}
			bool Isdublicate = _context.GlobalTabs.Any(c => c.Text == NewGlobalTab.Text && c.Title == NewGlobalTab.Title);

			if (Isdublicate)
			{
				ModelState.AddModelError("", "You cannot enter the same data again");
				return View();
			}

			_context.GlobalTabs.Add(NewGlobalTab);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Edit(int id)
		{
			if (id == 0) return NotFound();
			GlobalTab? globalTab = _context.GlobalTabs.FirstOrDefault(s => s.Id == id);
			if (globalTab is null) return NotFound();
			return View(globalTab);
		}

		[HttpPost]
		public IActionResult Edit(int id, GlobalTab edited)
		{
			if (id != edited.Id) return NotFound();
			GlobalTab? globalTab = _context.GlobalTabs.FirstOrDefault(s => s.Id == id);
			if (globalTab is null) return NotFound();
			bool duplicate = _context.GlobalTabs.Any(s => s.Text == edited.Text && s.Title == edited.Title && globalTab.Text != edited.Text && globalTab.Title != edited.Title);


			if (duplicate)
			{
				ModelState.AddModelError("Name", "This is now available");
				return View();
			}
			globalTab.Text = edited.Text;
			globalTab.Title = edited.Title;
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Details(int id)
		{
			if (id == 0) return NotFound();
			GlobalTab? globalTab = _context.GlobalTabs.FirstOrDefault(s => s.Id == id);
			return globalTab is null ? BadRequest() : View(globalTab);
		}



		public IActionResult Delete(int id)
		{
			if (id == 0) return NotFound();
			GlobalTab? globalTab = _context.GlobalTabs.FirstOrDefault(s => s.Id == id);

			if (globalTab is null) return NotFound();
			return View(globalTab);
		}

		[HttpPost]
		public IActionResult Delete(int id, GlobalTab deleted)
		{
			if (id != deleted.Id) return NotFound();
			GlobalTab? globalTab = _context.GlobalTabs.FirstOrDefault(s => s.Id == id);
			if (globalTab is null) return NotFound();
			_context.GlobalTabs.Remove(globalTab);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
	}
}
