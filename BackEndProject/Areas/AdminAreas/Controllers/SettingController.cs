using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Extension;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
    [Area("AdminAreas")]
    public class SettingController : Controller
    {
        private readonly ProductDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SettingController(ProductDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Setting> settings = _context.Settings.AsEnumerable();
            return View(settings);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Setting newSetting)
        {
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
                return View();
            }

            if (newSetting.Image != null && newSetting.Image.Length > 0)
            {
                var imagefolderPath = Path.Combine(_env.WebRootPath, "assets");
                newSetting.Value = await newSetting.Image.CreateImage(imagefolderPath, "images");
            }

            bool IsDuplicate = _context.Settings.Any(c => c.Key == newSetting.Key);

            if (IsDuplicate)
            {
                ModelState.AddModelError("", "You cannot enter the same data again");
                return View();
            }

            _context.Settings.Add(newSetting);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            Setting setting = _context.Settings.FirstOrDefault(s => s.Id == id);
            if (setting is null)
            {
                return BadRequest();
            }
            return View(setting);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Setting edited)
        {
            if (id != edited.Id) return NotFound();
            Setting setting = _context.Settings.FirstOrDefault(s => s.Id == id);
            if (!ModelState.IsValid) return View(setting);
            _context.Entry<Setting>(setting).CurrentValues.SetValues(edited);

            if (edited.Image is not null)
            {
                var imagefolderPath = Path.Combine(_env.WebRootPath, "assets");

                string filepath = Path.Combine(imagefolderPath, "images", setting.Value);
                ExtensionMethods.DeleteImage(filepath);
                setting.Value = await edited.Image.CreateImage(imagefolderPath, "images");
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            Setting? setting = _context.Settings.FirstOrDefault(c => c.Id == id);
            return setting is null ? BadRequest() : View(setting);
        }



        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Setting? setting = _context.Settings.FirstOrDefault(c => c.Id == id);

            if (setting is null) return NotFound();
            return View(setting);
        }

        [HttpPost]
        public IActionResult Delete(int id, Setting deleted)
        {
            if (id != deleted.Id) return NotFound();
            Setting? setting = _context.Settings.FirstOrDefault(c => c.Id == id);
            if (setting is null) return NotFound();
            _context.Settings.Remove(setting);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
