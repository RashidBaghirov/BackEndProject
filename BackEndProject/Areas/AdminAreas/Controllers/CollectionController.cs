using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
	[Area("AdminAreas")]
	[Authorize(Roles = "Admin,Moderator")]
	public class CollectionController : Controller
	{

		private readonly ProductDbContext _context;

		public CollectionController(ProductDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			IEnumerable<Collection> collections = _context.Collections.AsEnumerable();
			return View(collections);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Collection newCollection)
		{

			if (!ModelState.IsValid)
			{
				foreach (string message in ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
				{
					ModelState.AddModelError("", message);
				}
				return View();
			}
			bool Isdublicate = _context.Categories.Any(c => c.Name == newCollection.Name);

			if (Isdublicate)
			{
				ModelState.AddModelError("", "You cannot enter the same data again");
				return View();
			}
			_context.Collections.Add(newCollection);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Edit(int id)
		{
			if (id == 0) return NotFound();
			Collection? collection = _context.Collections.FirstOrDefault(c => c.Id == id);
			if (collection is null) return NotFound();
			return View(collection);
		}

		[HttpPost]
		public IActionResult Edit(int id, Collection editCollection)
		{
			if (id != editCollection.Id) return NotFound();
			Collection? collection = _context.Collections.FirstOrDefault(c => c.Id == id);
			if (collection is null) return NotFound();
			bool duplicate = _context.Collections.Any(c => c.Name == editCollection.Name && collection.Name != editCollection.Name);
			if (duplicate)
			{
				ModelState.AddModelError("Name", "This  category name is now available");
				return View();
			}
			collection.Name = editCollection.Name;
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Details(int id)
		{
			if (id == 0) return NotFound();
			Collection? collection = _context.Collections.FirstOrDefault(c => c.Id == id);
			return collection is null ? BadRequest() : View(collection);
		}



		public IActionResult Delete(int id)
		{
			if (id == 0) return NotFound();
			Collection? collection = _context.Collections.FirstOrDefault(c => c.Id == id);
			if (collection is null) return NotFound();
			return View(collection);
		}

		[HttpPost]
		public IActionResult Delete(int id, Collection deleteCollection)
		{
			if (id != deleteCollection.Id) return NotFound();
			Collection? collection = _context.Collections.FirstOrDefault(c => c.Id == id);
			if (collection is null) return NotFound();
			_context.Collections.Remove(collection);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
	}
}
