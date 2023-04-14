using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc;
using BackEndProject.Utilities.Extension;
using Microsoft.CodeAnalysis.Differencing;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
    [Area("AdminAreas")]

    public class ColorController : Controller
    {

        private readonly ProductDbContext _context;
        readonly IWebHostEnvironment _env;

        public ColorController(ProductDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Color> colors = _context.Colors.AsEnumerable();
            return View(colors);
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Color newColor)
        {
            if (newColor.Image is null)
            {
                ModelState.AddModelError("Image", "Please Select Image");
                return View();
            }
            if (!newColor.Image.IsValidFile("image/"))
            {
                ModelState.AddModelError("Image", "Please Select Image Tag");
                return View();
            }
            if (!newColor.Image.IsValidLength(2))
            {
                ModelState.AddModelError("Image", "Please Select Image which size max 2MB");
                return View();
            }
            if (newColor.Name is null)
            {
                ModelState.AddModelError("", "Note the Color name!");
                return View();
            }
            var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");
            newColor.ColorPath = await newColor.Image.CreateImage(imagefolderPath, "product-page");
            _context.Colors.Add(newColor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Color? color = _context.Colors.FirstOrDefault(c => c.Id == id);
            if (color is null) return NotFound();
            return View(color);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Color editcolor)
        {
            if (id != editcolor.Id) return NotFound();
            Color color = _context.Colors.FirstOrDefault(s => s.Id == id);
            if (!ModelState.IsValid) return View(color);
            _context.Entry<Color>(color).CurrentValues.SetValues(editcolor);

            if (editcolor.Image is not null)
            {
                var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");

                string filepath = Path.Combine(imagefolderPath, "product-page", color.ColorPath);
                ExtensionMethods.DeleteImage(filepath);
                color.ColorPath = await editcolor.Image.CreateImage(imagefolderPath, "product-page");
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            Color? color = _context.Colors.FirstOrDefault(c => c.Id == id);
            return color is null ? BadRequest() : View(color);
        }



        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Color? color = _context.Colors.FirstOrDefault(c => c.Id == id);

            if (color is null) return NotFound();
            return View(color);
        }

        [HttpPost]
        public IActionResult Delete(int id, Color deleteColor)
        {
            if (id != deleteColor.Id) return NotFound();
            Color? color = _context.Colors.FirstOrDefault(c => c.Id == id);
            if (color is null) return NotFound();
            _context.Colors.Remove(color);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }





    }
}
