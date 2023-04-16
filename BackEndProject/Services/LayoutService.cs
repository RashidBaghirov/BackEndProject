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


		public List<BasketItem>? GetBasketItems()
		{
			List<BasketItem> basket = _context.BasketItems.Include(p => p.ProductSizeColor.Product).ThenInclude(p => p.ProductImages).ToList();

			return basket;
		}
		public List<Product> GetProducts()
		{
			List<Product> products = _context.Products.Include(p => p.ProductImages).ToList();
			return products;
		}

		//public List<Basket> GetBasketItems()
		//{
		//	List<Basket> baskets = _context.Baskets.Include(b => b.BasketItems).ToList();

		//	return baskets;
		//}

		public List<CookiesBasketItemVM> GetBasketItem()
		{
			List<CookiesBasketItemVM> items = new List<CookiesBasketItemVM>();

			User member = null;

			if (_accessor.HttpContext.User.Identity.IsAuthenticated)
			{
				member = _userManager.Users.FirstOrDefault(x => x.UserName == _accessor.HttpContext.User.Identity.Name);
			}


			if (member == null)
			{
				var itemsStr = _accessor.HttpContext.Request.Cookies["Products"];

				if (itemsStr != null)
				{
					items = JsonConvert.DeserializeObject<List<CookiesBasketItemVM>>(itemsStr);

					foreach (var item in items)
					{
						Product product = _context.Products.Include(c => c.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
						if (product != null)
						{
							item.ProductId = product.Id;
							item.Price = product.Price;
							item.Quantity = product.AddCart.Quantity;
						}
					}
				}
			}
			else
			{
				List<BasketItem> basketItems = _context.BasketItems.Include(x => x.ProductSizeColor.Product).ThenInclude(x => x.ProductImages).Where(x => x.Basket.User.Id == member.Id).ToList();
				items = basketItems.Select(x => new CookiesBasketItemVM
				{
					ProductId = x.ProductSizeColor.ProductId,
					Quantity = x.SaleQuantity,
					ProductSizeColorId = x.ProductSizeColorId,
					Price = (decimal)x.ProductSizeColor.Product.DiscountPrice
				}).ToList();
			}

			return items;
		}
	}
}
