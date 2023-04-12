using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Controllers
{
	public class HomeController : Controller
	{
		private readonly ProductDbContext _context;

		public HomeController(ProductDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			List<Slider> slider = _context.Sliders.OrderBy(s => s.Order).ToList();
			return View(slider);
		}
	}
}