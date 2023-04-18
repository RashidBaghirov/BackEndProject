using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Comparer;
using BackEndProject.Utilities.Extension;
using BackEndProject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace BackEndProject.Controllers
{
	public class ProductController : Controller
	{
		private readonly ProductDbContext _context;
		private readonly UserManager<User> _userManager;

		public ProductController(ProductDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public IActionResult Index(int page = 1)
		{

			ViewBag.TotalPage = Math.Ceiling((double)_context.Products.Count() / 6);
			ViewBag.CurrentPage = page;

			ViewBag.ProductsAZ = _context.Products.Include(p => p.ProductImages)
													   .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color).Include(c => c.Collections)
														.AsNoTracking().Skip((page - 1) * 6).Take(6).OrderByDescending(p => p.Name).ToList();

			ViewBag.ProductsZA = _context.Products.Include(p => p.ProductImages)
										   .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color).Include(c => c.Collections)
										   .AsNoTracking().Skip((page - 1) * 6).Take(6).OrderBy(p => p.Name).ToList();

			ViewBag.ProductsLowToHigh = _context.Products.Include(p => p.ProductImages)
													 .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color).Include(c => c.Collections)
													 .AsNoTracking().Skip((page - 1) * 6).Take(6).OrderBy(p => p.Price).ToList();
			ViewBag.ProductsHighToLow = _context.Products.Include(p => p.ProductImages)
												 .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color).Include(c => c.Collections)
												 .AsNoTracking().Skip((page - 1) * 6).Take(6).OrderByDescending(p => p.Price).ToList();


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
							  Include(c => c.Comments).ThenInclude(u => u.User).
							  Include(p => p.GlobalTab)
							  .Include(p => p.ProductTags).ThenInclude(cu => cu.Tag)
							  .Include(p => p.ProductCategories)
							  .ThenInclude(pt => pt.Category)
							  .Include(p => p.Collections)
							  .AsSingleQuery()
							  .FirstOrDefault(p => p.Id == id);
			ViewBag.Products = ExtensionMethods.Related(products, product, id);
			ViewBag.Colors = _context.ProductSizeColors
						  .Where(psc => psc.ProductId == id)
						  .Select(psc => psc.Color)
						  .Distinct()
						  .Select(c => new { c.Id, c.Name })
						  .ToList();

			ViewBag.Sizes = _context.ProductSizeColors
							 .Where(psc => psc.ProductId == id)
							 .Select(psc => psc.Size)
							 .Distinct()
							 .Select(s => new { s.Id, s.Name })
							 .ToList();

			ViewBag.Comment = _context.Comments.ToList();

			if (product is null) return NotFound();
			return View(product);
		}

		public async Task<IActionResult> AddComment(int id, Comment newComment)
		{
			if (newComment.Text is null)
			{
				return RedirectToAction(nameof(Detail), new { id });
			}
			Product product = _context.Products.Include(c => c.Comments).FirstOrDefault(c => c.Id == id);
			User user = await _userManager.FindByNameAsync(User.Identity.Name);
			Comment comment = new()
			{
				Text = newComment.Text,
				User = user,
				Date = newComment.Date,
				ProductId = newComment.ProductId,
				Product = product
			};
			user.Comments.Add(comment);
			product.Comments.Add(comment);
			_context.Comments.Add(comment);
			_context.SaveChanges();
			return RedirectToAction(nameof(Detail), new { id });
		}


		//Edite-i yazdiqdan sonra menasiz geldi eger bir Comment sehfdise silsin yeniden yazsinda :D
		//public async Task<IActionResult> Edited(Comment edited, int id)
		//{
		//	if (edited.Text is null)
		//	{
		//		return RedirectToAction(nameof(Detail), new { id });
		//	}
		//	User user = await _userManager.FindByNameAsync(User.Identity.Name);
		//	Comment comment = _context.Comments.FirstOrDefault(c => c.Id == id);
		//	comment.Text = edited.Text;
		//	comment.ProductId = edited.ProductId;
		//	int Id = comment.ProductId;
		//	_context.SaveChanges();
		//	return RedirectToAction(nameof(Detail), new { Id });
		//}

		public async Task<IActionResult> DeleteComment(int commentId)
		{
			Comment comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
			if (comment is not null)
			{
				User user = await _userManager.FindByNameAsync(User.Identity.Name);
				if (comment.User == user)
				{
					_context.Comments.Remove(comment);
					_context.SaveChanges();
				}
			}
			return RedirectToAction(nameof(Detail), new { id = comment.ProductId });
		}

	}
}
