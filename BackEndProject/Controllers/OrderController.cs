using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.ViewModel;
using BackEndProject.ViewModel.Basket;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P230_Pronia.ViewModels.Cookies;
using System.Diagnostics.Metrics;

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
			CookiesBasketVM basket = new();
			if (cookies is null)
			{
				CookiesBasketItemVM item = new CookiesBasketItemVM
				{
					Id = product.Id,
					Price = product.Price,
					Quantity = 1

				};
				basket.CookiesBasketItems.Add(item);
				basket.TotalPrice = product.Price;
			}
			else
			{
				basket = JsonConvert.DeserializeObject<CookiesBasketVM>(cookies);
				CookiesBasketItemVM existed = basket.CookiesBasketItems.Find(c => c.Id == id);
				if (existed is null)
				{
					CookiesBasketItemVM newitem = new CookiesBasketItemVM
					{
						Id = product.Id,
						Price = product.Price,
						Quantity = 1

					};
					basket.CookiesBasketItems.Add(newitem);
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


		public async Task<IActionResult> WishList()
		{
			List<WishListItemVM> items = new();

			User? user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user is null)
			{
				var itemsStr = HttpContext.Request.Cookies["WishList"];

				if (itemsStr is not null)
				{
					items = JsonConvert.DeserializeObject<List<WishListItemVM>>(itemsStr);

					foreach (var item in items)
					{
						Product product = _context.Products.Include(c => c.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);

						if (product != null)
						{
							item.Name = product.Name;
							item.Price = (double)product.DiscountPrice;
							item.Image = product.ProductImages.FirstOrDefault(x => x.IsMain == true)?.Path;
						}
					}
				}
			}
			else
			{
				List<WishListItem> wishlistItems = _context.WishListItems.Include(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.UserId == user.Id).ToList();
				items = wishlistItems.Select(x => new WishListItemVM
				{
					ProductId = x.ProductId,
					Image = x.Product.ProductImages.FirstOrDefault(bi => bi.IsMain == true)?.Path,
					Name = x.Product.Name,
					Price = (double)x.Product.DiscountPrice
				}).ToList();

				// _context.WishListItems.Add(items); // burada hata verir
				// değiştirilmesi gereken kod şu şekildedir:
				foreach (var item in items)
				{
					var wishListItem = new WishListItem
					{
						UserId = user.Id,
						ProductId = item.ProductId
					};
					_context.WishListItems.Add(wishListItem);
				}

				_context.SaveChanges(); // save işlemini burada yapabilirsiniz
			}
			return RedirectToAction("Index", "Home");
		}

	}
}
