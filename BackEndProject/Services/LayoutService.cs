using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.ViewModel.Basket;
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

		public LayoutService(ProductDbContext context, IHttpContextAccessor accessor)
		{
			_context = context;
			_accessor = accessor;
		}

		public Dictionary<string, string> GetSettings()
		{
			Dictionary<string, string> settings = _context.Settings.ToDictionary(s => s.Key, s => s.Value);

			return settings;
		}


		public CookiesBasketVM? GetBasket()
		{
			var cookies = _accessor.HttpContext.Request.Cookies["basket"];
			CookiesBasketVM basket = new();
			if (cookies is not null)
			{
				basket = JsonConvert.DeserializeObject<CookiesBasketVM>(cookies);
				foreach (CookiesBasketItemVM item in basket.CookiesBasketItems)
				{
					Product plant = _context.Products.FirstOrDefault(p => p.Id == item.Id);
					if (plant is null)
					{
						basket.CookiesBasketItems.Remove(item);
						basket.TotalPrice -= item.Quantity * item.Price;
					}
				}
			}
			return basket;

		}

		public List<Product> GetProducts()
		{
			List<Product> products = _context.Products.Include(p => p.ProductImages).ToList();
			return products;
		}
	}



}
