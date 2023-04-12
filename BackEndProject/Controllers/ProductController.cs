using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
		public IActionResult Index()
		{


			ViewBag.Products = _context.Products.Include(p => p.ProductImages).Take(8).ToList();
			return View();
		}



		public IActionResult Detail()
		{

			//Product? product = _context.Products
			//				  .Include(p => p.ProductImages)
			//				  .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
			//				  .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
			//				  .Include(p => p.Instructions).
			//				  Include(p => p.GlobalTab)
			//				  .Include(p => p.ProductTags).ThenInclude(cu => cu.Tag)
			//				  .Include(p => p.ProductCategories)
			//				  .ThenInclude(pt => pt.Category)
			//				  .Include(p => p.Instructions)
			//				  .AsSingleQuery()
			//				  .FirstOrDefault(p => p.Id == id);
			return View();
		}
	}
}
