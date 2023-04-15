using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
	[Area("AdminAreas")]
	[Authorize(Roles = "Admin,Moderator")]
	public class SizeController : Controller
	{

		private readonly ProductDbContext _context;
		public SizeController(ProductDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			IEnumerable<Size> size = _context.Sizes.AsEnumerable();
			return View(size);
		}


		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Size newSize)
		{

			if (!ModelState.IsValid)
			{
				foreach (string message in ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
				{
					ModelState.AddModelError("", message);
				}
				return View();
			}
			bool Isdublicate = _context.Sizes.Any(c => c.Name == newSize.Name);

			if (Isdublicate)
			{
				ModelState.AddModelError("", "You cannot enter the same data again");
				return View();
			}
			_context.Sizes.Add(newSize);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Edit(int id)
		{
			if (id == 0) return NotFound();
			Size? size = _context.Sizes.FirstOrDefault(s => s.Id == id);
			if (size is null) return NotFound();
			return View(size);
		}

		[HttpPost]
		public IActionResult Edit(int id, Size editsize)
		{
			if (id != editsize.Id) return NotFound();
			Size? size = _context.Sizes.FirstOrDefault(s => s.Id == id);
			if (size is null) return NotFound();
			bool duplicate = _context.Sizes.Any(s => s.Name == editsize.Name && size.Name != editsize.Name);
			if (duplicate)
			{
				ModelState.AddModelError("Name", "This size name is now available");
				return View();
			}
			size.Name = editsize.Name;
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Details(int id)
		{
			if (id == 0) return NotFound();
			Size? size = _context.Sizes.FirstOrDefault(s => s.Id == id);
			return size is null ? BadRequest() : View(size);
		}



		public IActionResult Delete(int id)
		{
			if (id == 0) return NotFound();
			Size? size = _context.Sizes.FirstOrDefault(s => s.Id == id);
			if (size is null) return NotFound();
			return View(size);
		}

		[HttpPost]
		public IActionResult Delete(int id, Size deletesize)
		{
			if (id != deletesize.Id) return NotFound();
			Size? size = _context.Sizes.FirstOrDefault(s => s.Id == id);
			if (size is null) return NotFound();
			_context.Sizes.Remove(size);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
	}
}
