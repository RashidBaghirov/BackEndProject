using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
    [Area("AdminAreas")]

    public class SliderController : Controller
    {
        readonly ProductDbContext _context;
        readonly IWebHostEnvironment _env;

        public SliderController(ProductDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Slider> sliders = _context.Sliders.AsEnumerable();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slider newSlider)
        {
            if (newSlider.Image is null)
            {
                ModelState.AddModelError("Image", "Please Select Image");
                return View();
            }
            if (!newSlider.Image.IsValidFile("image/"))
            {
                ModelState.AddModelError("Image", "Please Select Image Tag");
                return View();
            }
            if (!newSlider.Image.IsValidLength(2))
            {
                ModelState.AddModelError("Image", "Please Select Image which size max 2MB");
                return View();
            }
            if (!Imports(newSlider))
            {
                return View();
            }
            var maxOrder = await _context.Sliders.OrderByDescending(s => s.Order).Select(s => s.Order).FirstOrDefaultAsync();
            var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");

            newSlider.ImagePath = await newSlider.Image.CreateImage(imagefolderPath, "slider");
            newSlider.Order = (byte)(maxOrder + 1);
            _context.Sliders.Add(newSlider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            Slider slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null)
            {
                return BadRequest();
            }
            return View(slider);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Slider edited)
        {
            if (id != edited.Id) return NotFound();
            Slider slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (!ModelState.IsValid) return View(slider);
            _context.Entry<Slider>(slider).CurrentValues.SetValues(edited);

            if (edited.Image is not null)
            {
                var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");
                string filepath = Path.Combine(imagefolderPath, "slider", edited.ImagePath);
                ExtensionMethods.DeleteImage(filepath);
                slider.ImagePath = await edited.Image.CreateImage(imagefolderPath, "slider");
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            Slider? slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            return slider is null ? BadRequest() : View(slider);
        }

        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Slider? slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null) return NotFound();
            return View(slider);
        }

        [HttpPost]
        public IActionResult Delete(int id, Slider deleteslider)
        {
            if (id != deleteslider.Id) return NotFound();
            Slider? slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null) return NotFound();
            var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");

            string filepath = Path.Combine(imagefolderPath, "slider", slider.ImagePath);
            ExtensionMethods.DeleteImage(filepath);
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        bool Imports(Slider newSlider)
        {
            if (newSlider.Title is null)
            {
                ModelState.AddModelError("", "Note the Title!");
                return false;
            }
            if (newSlider.Buttontitle is null)
            {
                ModelState.AddModelError("", "Note the ButtonTitle!");
                return false;
            }

            if (newSlider.Order is null)
            {
                ModelState.AddModelError("", "Note the Order!");
                return false;
            }

            return true;
        }

    }
}
