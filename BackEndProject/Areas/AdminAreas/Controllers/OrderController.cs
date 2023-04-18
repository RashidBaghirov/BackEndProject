using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
    [Area("AdminAreas")]
    [Authorize(Roles = "Admin,Moderator")]
    public class OrderController : Controller
    {
        private readonly ProductDbContext _context;

        public OrderController(ProductDbContext context, IWebHostEnvironment env)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Order> orders = _context.Orders.ToList();
            return View(orders);
        }

        public IActionResult Edit(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).Include(p => p.Basket).ThenInclude(pt => pt.BasketItems).Include(p => p.Basket.BasketItems).ThenInclude(p => p.ProductSizeColor.Product).Include(p => p.Basket.BasketItems).ThenInclude(p => p.ProductSizeColor.Product.ProductImages).FirstOrDefault(x => x.Id == id);

            if (order is null) return NotFound();
            return View(order);
        }


        public IActionResult Accept(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);

            if (order is null) return NotFound();
            order.Status = BackEndProject.Utilities.Enum.OrderStatus.Accepted;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Reject(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order is null) return NotFound();

            order.Status = BackEndProject.Utilities.Enum.OrderStatus.Rejected;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}
