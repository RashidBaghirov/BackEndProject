using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.ViewModel.Basket;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BackEndProject.Controllers
{
	public class OrderController : Controller
	{
		private readonly ProductDbContext _context;
		private readonly UserManager<User> _userManager;

		public OrderController(ProductDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		[HttpPost]
		public async Task<IActionResult> AddBaskets(int productId, Product basketProduct)
		{
			if (User.Identity.IsAuthenticated)
			{
				ProductSizeColor? plant = _context.ProductSizeColors.Include(p => p.Product).FirstOrDefault(p => p.ProductId == productId && p.SizeId == basketProduct.AddCart.SizeId && p.ColorId == basketProduct.AddCart.ColorId);
				if (plant is null) return NotFound();

				User? user = await _userManager.FindByNameAsync(User.Identity.Name);
				Basket? userActiveBasket = _context.Baskets
												.Include(b => b.User)
												   .FirstOrDefault(b => b.User.Id == user.Id && !b.IsOrdered) ?? new Basket();

				BasketItem item = new()
				{
					ProductSizeColor = plant,
					SaleQuantity = basketProduct.AddCart.Quantity,
					UnitPrice = plant.Product.Price,
					Basket = userActiveBasket
				};
				userActiveBasket.BasketItems.Add(item);
				userActiveBasket.User = user;
				userActiveBasket.TotalPrice = userActiveBasket.BasketItems.Sum(p => p.SaleQuantity * p.UnitPrice);
				//return Json(userActiveBasket);
				_context.Baskets.Add(userActiveBasket);
				_context.SaveChangesAsync();
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return RedirectToAction("Login", "Account");
			}
		}




		public IActionResult AddBasket(int id)
		{
			if (id <= 0) return NotFound();
			Product product = _context.Products.FirstOrDefault(p => p.Id == id);
			if (product is null) return NotFound();
			var cookies = HttpContext.Request.Cookies["basket"];
			BasketVM basket = new();
			if (cookies is null)
			{
				BasketItemVM item = new BasketItemVM
				{
					Id = product.Id,
					Price = product.Price,
					Quantity = 1

				};
				basket.BasketItemVMs.Add(item);
				basket.TotalPrice = product.Price;
			}
			else
			{
				basket = JsonConvert.DeserializeObject<BasketVM>(cookies);
				BasketItemVM existed = basket.BasketItemVMs.Find(c => c.Id == id);
				if (existed is null)
				{
					BasketItemVM newitem = new BasketItemVM
					{
						Id = product.Id,
						Price = product.Price,
						Quantity = 1

					};
					basket.BasketItemVMs.Add(newitem);
					basket.TotalPrice += newitem.Price;

				}
				else
				{
					existed.Quantity++;
					basket.TotalPrice += existed.Price;
				}

			}
			var basketstr = JsonConvert.SerializeObject(basket);
			HttpContext.Response.Cookies.Append("basket", basketstr);
			return RedirectToAction("Index", "Home");
		}

		public IActionResult RemoveBasketItem(int id)
		{
			var cookies = HttpContext.Request.Cookies["basket"];
			var basket = JsonConvert.DeserializeObject<BasketVM>(cookies);
			var item = basket.BasketItemVMs.FirstOrDefault(i => i.Id == id);
			if (item is not null)
			{
				basket.BasketItemVMs.Remove(item);
				basket.TotalPrice -= item.Price * item.Quantity;

				var basketStr = JsonConvert.SerializeObject(basket);
				HttpContext.Response.Cookies.Append("basket", basketStr);
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
