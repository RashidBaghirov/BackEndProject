using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BackEndProject.Controllers
{
    public class ContactController : Controller
    {
        private readonly ProductDbContext _context;

        public ContactController(ProductDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            Dictionary<string, string> settings = _context.Settings.ToDictionary(s => s.Key, s => s.Value);
            return View(settings);
        }
    }
}
