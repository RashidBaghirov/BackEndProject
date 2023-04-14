using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
	[Area("AdminAreas")]
	public class TagController : Controller
	{
		private readonly ProductDbContext _context;

		public TagController(ProductDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			IEnumerable<Tag> tag = _context.Tags.AsEnumerable();
			return View(tag);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Tag newtag)
		{

			if (!ModelState.IsValid)
			{
				foreach (string message in ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage))
				{
					ModelState.AddModelError("", message);
				}
			}
			bool isdublicate = _context.Tags.Any(t => t.Name == newtag.Name);
			if (isdublicate)
			{
				ModelState.AddModelError("", "You cannot enter the same data again");
				return View();
			}
			_context.Tags.Add(newtag);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}


		public IActionResult Edit(int id)
		{
			if (id == 0) return NotFound();
			Tag? tag = _context.Tags.FirstOrDefault(t => t.Id == id);
			if (tag is null) return NotFound();
			return View(tag);
		}

		[HttpPost]
		public IActionResult Edit(int id, Tag edittag)
		{
			if (id != edittag.Id) return NotFound();
			Tag? tag = _context.Tags.FirstOrDefault(t => t.Id == id);
			if (tag is null) return NotFound();
			bool duplicate = _context.Tags.Any(t => t.Name == edittag.Name && tag.Name != edittag.Name);
			if (duplicate)
			{
				ModelState.AddModelError("Name", $"This name is now available");
				return View();
			}
			tag.Name = edittag.Name;
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Details(int id)
		{
			if (id == 0) return NotFound();
			Tag? tag = _context.Tags.FirstOrDefault(t => t.Id == id);
			return tag is null ? BadRequest() : View(tag);
		}



		public IActionResult Delete(int id)
		{
			if (id == 0) return NotFound();
			Tag? tag = _context.Tags.FirstOrDefault(t => t.Id == id);
			if (tag is null) return NotFound();
			return View(tag);
		}

		[HttpPost]
		public IActionResult Delete(int id, Tag deleteTag)
		{
			if (id != deleteTag.Id) return NotFound();
			Tag? tag = _context.Tags.FirstOrDefault(t => t.Id == id);
			if (tag is null) return NotFound();
			_context.Remove(tag);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
	}
}
