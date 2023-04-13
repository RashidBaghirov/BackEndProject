using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Comparer;
using BackEndProject.Utilities.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace BackEndProject.Controllers
{
	public class ProductController : Controller
	{
		private readonly ProductDbContext _context;

		public ProductController(ProductDbContext context)
		{
			_context = context;
		}
		public IActionResult Index(int page = 1)
		{

			ViewBag.TotalPage = Math.Ceiling((double)_context.Products.Count() / 6);
			ViewBag.CurrentPage = page;
			ViewBag.Products = _context.Products.Include(p => p.ProductImages)
													   .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color).Include(c => c.Collections)
														.AsNoTracking().Skip((page - 1) * 6).Take(6).ToList();

			return View();
		}



		public IActionResult Detail(int id)
		{
			if (id <= 0) return NotFound();
			IQueryable<Product> products = _context.Products.AsNoTracking().AsQueryable();

			Product? product = products
							  .Include(p => p.ProductImages)
							  .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
							  .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
							  .Include(p => p.Instructions).
							  Include(p => p.GlobalTab)
							  .Include(p => p.ProductTags).ThenInclude(cu => cu.Tag)
							  .Include(p => p.ProductCategories)
							  .ThenInclude(pt => pt.Category)
							  .Include(p => p.Collections)
							  .AsSingleQuery()
							  .FirstOrDefault(p => p.Id == id);
			ViewBag.Products = ExtensionMethods.Related(products, product, id);
			ViewBag.Colors = _context.Colors.ToList();
			ViewBag.Sizes = _context.Sizes.ToList();
			if (product is null) return NotFound();
			return View(product);
		}



	}
}
