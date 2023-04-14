using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
    [Area("AdminAreas")]

    public class ProductsController : Controller
    {
        private readonly ProductDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(ProductDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Products.Count() / 5);
            ViewBag.CurrentPage = page;

            IEnumerable<Product> products = _context.Products.Include(p => p.ProductImages)
                                                        .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
                                                        .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
                                                         .AsNoTracking().Skip((page - 1) * 5).Take(5).AsEnumerable();
            return View(products);
        }


        public IActionResult Create()
        {
            ViewBag.GlobalTabs = _context.GlobalTabs.AsEnumerable();
            ViewBag.Instruction = _context.Instructions.AsEnumerable();
            ViewBag.Collection = _context.Collections.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();
            ViewBag.Sizes = _context.Sizes.AsEnumerable();
            ViewBag.Colors = _context.Colors.AsEnumerable();
            return View();

        }




    }


}
