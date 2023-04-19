using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P230_Pronia.ViewModels.Cookies;

namespace BackEndProject.Services
{
	public class LayoutService
	{
		private readonly ProductDbContext _context;
		private readonly IHttpContextAccessor _accessor;
		private readonly UserManager<User> _userManager;

		public LayoutService(ProductDbContext context, IHttpContextAccessor accessor, UserManager<User> userManager)
		{
			_context = context;
			_accessor = accessor;
			_userManager = userManager;
		}

		public Dictionary<string, string> GetSettings()
		{
			Dictionary<string, string> settings = _context.Settings.ToDictionary(s => s.Key, s => s.Value);

			return settings;
		}


		public List<Product> GetProducts()
		{
			List<Product> products = _context.Products.Include(p => p.ProductImages).ToList();
			return products;
		}
		public List<BasketItem>? GetBasketItems()
		{
			User user = new();

			if (_accessor.HttpContext.User.Identity.IsAuthenticated)
			{
				user = _userManager.Users.FirstOrDefault(x => x.UserName == _accessor.HttpContext.User.Identity.Name);
			}

			List<BasketItem> basket = _context.BasketItems.Include(x => x.ProductSizeColor.Product).ThenInclude(x => x.ProductImages).Where(x => x.Basket.User.Id == user.Id).ToList();

			return basket;
		}


		public List<BasketItemVM> GetBasketItem()
		{
			List<BasketItemVM> items = new();

			User user = new();

			if (_accessor.HttpContext.User.Identity.IsAuthenticated)
			{
				user = _userManager.Users.FirstOrDefault(x => x.UserName == _accessor.HttpContext.User.Identity.Name);
			}

			List<BasketItem> basketItems = _context.BasketItems.Include(x => x.ProductSizeColor.Product).ThenInclude(x => x.ProductImages).Where(x => x.Basket.User.Id == user.Id).ToList();
			items = basketItems.Select(x => new BasketItemVM
			{
				ProductId = x.ProductSizeColor.ProductId,
				Quantity = x.SaleQuantity,
				ProductSizeColorId = x.ProductSizeColorId,
				Price = (decimal)x.ProductSizeColor.Product.DiscountPrice
			}).ToList();


			return items;
		}
	}
}
