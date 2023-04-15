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
                ProductSizeColor? productSizeColor = _context.ProductSizeColors
                    .Include(p => p.Product)
                    .FirstOrDefault(p => p.ProductId == productId && p.SizeId == basketProduct.AddCart.SizeId && p.ColorId == basketProduct.AddCart.ColorId);

                if (productSizeColor is null)
                    return NotFound();

                User? user = await _userManager.FindByNameAsync(User.Identity.Name);

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

                BasketItem item = new()
                {
                    ProductSizeColor = productSizeColor,
                    SaleQuantity = basketProduct.AddCart.Quantity,
                    UnitPrice = productSizeColor.Product.Price,
                    Basket = userActiveBasket
                };
                userActiveBasket.BasketItems.Add(item);

                userActiveBasket.TotalPrice = userActiveBasket.BasketItems.Sum(p => p.SaleQuantity * p.UnitPrice);

                _context.SaveChanges();

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

    }
}
