using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Enum;
using BackEndProject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P230_Pronia.ViewModels.Cookies;
using System.Diagnostics.Metrics;
using System.Security.Claims;

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

		//[HttpPost]
		//public async Task<IActionResult> AddBaskets(int productId, Product basketProduct)
		//{
		//	if (User.Identity.IsAuthenticated)
		//	{
		//		ProductSizeColor? productSizeColor = _context.ProductSizeColors
		//			.Include(p => p.Product)
		//			.FirstOrDefault(p => p.ProductId == productId && p.SizeId == basketProduct.AddCart.SizeId && p.ColorId == basketProduct.AddCart.ColorId);

		//		if (productSizeColor is null)
		//			return NotFound();

		//		User? user = await _userManager.FindByNameAsync(User.Identity.Name);

		//		Basket userActiveBasket = _context.Baskets
		//			.Include(b => b.User)
		//			.Include(b => b.BasketItems)
		//			.ThenInclude(i => i.ProductSizeColor)
		//			.FirstOrDefault(b => b.User.Id == user.Id && !b.IsOrdered);

		//		if (userActiveBasket is null)
		//		{
		//			userActiveBasket = new Basket()
		//			{
		//				User = user,
		//				BasketItems = new List<BasketItem>(),
		//			};
		//			_context.Baskets.Add(userActiveBasket);
		//		}

		//		BasketItem item = userActiveBasket.BasketItems.FirstOrDefault(i => i.ProductSizeColor == productSizeColor);

		//		if (item != null)
		//		{
		//			item.SaleQuantity += basketProduct.AddCart.Quantity;
		//		}
		//		else
		//		{
		//			item = new BasketItem
		//			{
		//				ProductSizeColor = productSizeColor,
		//				SaleQuantity = basketProduct.AddCart.Quantity,
		//				UnitPrice = productSizeColor.Product.Price,
		//				Basket = userActiveBasket
		//			};
		//			userActiveBasket.BasketItems.Add(item);
		//		}

		//		userActiveBasket.TotalPrice = userActiveBasket.BasketItems.Sum(p => p.SaleQuantity * p.UnitPrice);

		//		_context.SaveChanges();

		//		return RedirectToAction("Index", "Home");
		//	}
		//	else
		//	{
		//		return RedirectToAction("Login", "Account");
		//	}
		//}





		//public IActionResult AddBasket(int id)
		//{
		//	if (id <= 0) return NotFound();
		//	Product product = _context.Products.FirstOrDefault(p => p.Id == id);
		//	if (product is null) return NotFound();
		//	var cookies = HttpContext.Request.Cookies["basket"];
		//	CookiesBasketVM basket = new();
		//	if (cookies is null)
		//	{
		//		CookiesBasketItemVM item = new CookiesBasketItemVM
		//		{
		//			Id = product.Id,
		//			Price = product.Price,
		//			Quantity = 1

		//		};
		//		basket.CookiesBasketItems.Add(item);
		//		basket.TotalPrice = product.Price;
		//	}
		//	else
		//	{
		//		basket = JsonConvert.DeserializeObject<CookiesBasketVM>(cookies);
		//		CookiesBasketItemVM existed = basket.CookiesBasketItems.Find(c => c.Id == id);
		//		if (existed is null)
		//		{
		//			CookiesBasketItemVM newitem = new CookiesBasketItemVM
		//			{
		//				Id = product.Id,
		//				Price = product.Price,
		//				Quantity = 1

		//			};
		//			basket.CookiesBasketItems.Add(newitem);
		//			basket.TotalPrice += newitem.Price;

		//		}
		//		else
		//		{
		//			existed.Quantity++;
		//			basket.TotalPrice += existed.Price;
		//		}

		//	}
		//	var basketstr = JsonConvert.SerializeObject(basket);
		//	HttpContext.Response.Cookies.Append("basket", basketstr);
		//	return RedirectToAction("Index", "Home");
		//}
		//public IActionResult RemoveBasketItem(int id)
		//{
		//	var cookies = HttpContext.Request.Cookies["basket"];
		//	var basket = JsonConvert.DeserializeObject<CookiesBasketVM>(cookies);
		//	var item = basket.CookiesBasketItems.FirstOrDefault(i => i.Id == id);
		//	if (item is not null)
		//	{
		//		basket.CookiesBasketItems.Remove(item);
		//		basket.TotalPrice -= item.Price * item.Quantity;

		//		var basketStr = JsonConvert.SerializeObject(basket);
		//		HttpContext.Response.Cookies.Append("basket", basketStr);
		//	}

		//	return RedirectToAction("Index", "Home");
		//}

		public async Task<IActionResult> Addbaskets(int productId, Product basketProduct)
		{
			ProductSizeColor? productSizeColor = _context.ProductSizeColors
				.Include(p => p.Product)
				.FirstOrDefault(p => p.ProductId == productId && p.SizeId == basketProduct.AddCart.SizeId && p.ColorId == basketProduct.AddCart.ColorId);

			if (productSizeColor is null) return NotFound();

			User? user = new();

			if (User.Identity.IsAuthenticated)
			{
				user = await _userManager.FindByNameAsync(User.Identity.Name);
			}
			else
			{
				return RedirectToAction("Login", "Account");
			}
			BasketItem item = _context.BasketItems.FirstOrDefault(i => i.ProductSizeColor == productSizeColor);
			Basket userActiveBasket = _context.Baskets
				.Include(b => b.User)
				.Include(b => b.BasketItems)
				.ThenInclude(i => i.ProductSizeColor)
				.FirstOrDefault(b => b.User.Id == user.Id && !b.IsOrdered);

			if (userActiveBasket is null)
			{
				userActiveBasket = new Basket()
				{
					User = user,
					BasketItems = new List<BasketItem>(),
				};
				_context.Baskets.Add(userActiveBasket);
			}

			BasketItem items = userActiveBasket.BasketItems.FirstOrDefault(i => i.ProductSizeColor == productSizeColor);

			if (items is not null)
			{
				items.SaleQuantity += basketProduct.AddCart.Quantity;
			}
			else
			{
				items = new BasketItem
				{
					ProductSizeColor = productSizeColor,
					SaleQuantity = basketProduct.AddCart.Quantity,
					UnitPrice = (decimal)productSizeColor.Product.DiscountPrice,
					Basket = userActiveBasket
				};
				userActiveBasket.BasketItems.Add(items);
			}
			userActiveBasket.TotalPrice = userActiveBasket.BasketItems.Sum(p => p.SaleQuantity * p.UnitPrice);
			await _context.SaveChangesAsync();
			return RedirectToAction("index", "home");
		}

		public async Task<IActionResult> RemoveBasketItem(int basketItemId)
		{
			User? user = null; if (User.Identity.IsAuthenticated)
			{
				user = await _userManager.FindByNameAsync(User.Identity.Name);
			}
			BasketItem item = _context.BasketItems.FirstOrDefault(i => i.Id == basketItemId);

			if (item is not null)
			{
				Basket userActiveBasket = _context.Baskets
					.Include(b => b.User)
					.Include(b => b.BasketItems)
					.ThenInclude(i => i.ProductSizeColor)
					.FirstOrDefault(b => b.User.Id == user.Id && !b.IsOrdered);
				if (userActiveBasket is not null)
				{
					userActiveBasket.BasketItems.Remove(item);
					userActiveBasket.TotalPrice = userActiveBasket.BasketItems.Sum(p => p.SaleQuantity * p.UnitPrice);

					await _context.SaveChangesAsync();
				}
			}
			return RedirectToAction("index", "home");
		}




		public IActionResult Index()
		{
			if (!User.Identity.IsAuthenticated)
			{
				return View(new List<WishListItem>());
			}

			var userId = _userManager.GetUserId(User);

			var wishListItems = _context.WishListItems
				.Include(wli => wli.Product)
				.ThenInclude(p => p.ProductImages)
				.Where(wli => wli.UserId == userId)
				.ToList();

			if (wishListItems.Count == 0)
			{
				return View(new List<WishListItem>());
			}

			return View(wishListItems);
		}

		public async Task<IActionResult> AddToWishList(int productId)
		{
			Product product = await _context.Products.FindAsync(productId);

			if (product is null)
			{
				return NotFound();
			}

			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}

			User user = await _userManager.FindByNameAsync(User.Identity.Name);

			WishListItem userWishlistItem = await _context.WishListItems
				.FirstOrDefaultAsync(x => x.UserId == user.Id && x.ProductId == productId);

			if (userWishlistItem is null)
			{
				userWishlistItem = new WishListItem
				{
					UserId = user.Id,
					ProductId = productId
				};
				_context.WishListItems.Add(userWishlistItem);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}


		public async Task<IActionResult> RemoveFromWishList(int wishListItemId)
		{
			User user = await _userManager.FindByNameAsync(User.Identity.Name);
			WishListItem wishListItem = await _context.WishListItems
				.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == wishListItemId);
			if (wishListItem is null)
			{
				return NotFound();
			}
			_context.WishListItems.Remove(wishListItem);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Checkout()
		{
			CheckOutVM checkoutVM = new();

			User user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user is null) return View();

			checkoutVM.Email = user.Email;
			checkoutVM.FullName = user.FullName;
			checkoutVM.Phone = user.PhoneNumber;

			checkoutVM.BasketItemVMs = _context.BasketItems.Include(x => x.ProductSizeColor.Product).Where(x => x.Basket.User.Id == user.Id)
														   .Select(x => new BasketItemVM
														   {
															   ProductId = x.ProductSizeColor.ProductId,
															   ProductSizeColorId = x.ProductSizeColorId,
															   Quantity = x.SaleQuantity,
															   Price = x.UnitPrice
														   }).ToList();

			decimal totalPrice = 0;

			foreach (var item in checkoutVM.BasketItemVMs)
			{
				totalPrice += item.Quantity * item.Price;
			}

			checkoutVM.TotalPrice = totalPrice;

			ViewBag.Products = _context.Products.Include(p => p.ProductImages).ToList();
			return View(checkoutVM);
		}


		[HttpPost]
		public async Task<IActionResult> CheckOut(CheckOutVM model)
		{
			var order = new Order
			{
				FullName = model.FullName,
				Email = model.Email,
				Phone = model.Phone,
				Address = model.Address,
				Note = model.Note,
				CreatedAt = DateTime.Now,
				Status = model.OrderStatus,
				TotalPrice = 0,
				OrderItems = new List<OrderItem>()
			};
			decimal totalPrice = 0;
			foreach (BasketItemVM basketItem in model.BasketItemVMs)
			{
				ProductSizeColor? productSizeColor = await _context.ProductSizeColors
					.Include(p => p.Product)
					.FirstOrDefaultAsync(psc => psc.Id == basketItem.ProductSizeColorId);

				if (productSizeColor == null)
				{
					ModelState.AddModelError("ProductSizeColorId", "Product size color does not exist.");
					return View(model);
				}

				var orderItem = new OrderItem
				{
					SaleQuantity = basketItem.Quantity,
					UnitPrice = productSizeColor.Product.Price,
					ProductSizeColorId = basketItem.ProductSizeColorId,
					ProductSizeColor = productSizeColor,
				};
				order.OrderItems.Add(orderItem);
				decimal itemTotalPrice = orderItem.UnitPrice * orderItem.SaleQuantity;
				totalPrice += itemTotalPrice;
			}
			order.TotalPrice = totalPrice;
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index", "Home");
		}



	}
}
