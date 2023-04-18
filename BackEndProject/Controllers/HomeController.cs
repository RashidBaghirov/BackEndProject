using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
            ViewBag.Products = _context.Products.Include(p => p.ProductImages)
                                                       .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color).Include(c => c.Collections).OrderByDescending(p => p.Id)
                                                        .AsNoTracking().Take(6).ToList();
            return View(slider);
        }
        public IActionResult Search(string search)
        {
            var query = _context.Products.Include(p => p.ProductImages).AsQueryable().Where(x => x.Name.Contains(search));
            List<Product> products = query.OrderByDescending(x => x.Id).Take(3).ToList();
            return PartialView("_SearchPartial", products);
        }

    }
}