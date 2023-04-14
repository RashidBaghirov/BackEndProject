using BackEndProject.DAL;
using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
    [Area("AdminAreas")]

    public class InstructionController : Controller
    {
        private readonly ProductDbContext _context;
        public InstructionController(ProductDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Instruction> instuctions = _context.Instructions.AsEnumerable();
            return View(instuctions);
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Instruction newInstruction)
        {

            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
                return View();
            }
            bool Isdublicate = _context.Instructions.Any(c => c.QualityDetails == newInstruction.QualityDetails && c.Lining == newInstruction.Lining && c.Only == newInstruction.Only && c.Clean == newInstruction.Clean);

            if (Isdublicate)
            {
                ModelState.AddModelError("", "You cannot enter the same data again");
                return View();
            }

            _context.Instructions.Add(newInstruction);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Instruction? instruction = _context.Instructions.FirstOrDefault(s => s.Id == id);
            if (instruction is null) return NotFound();
            return View(instruction);
        }

        [HttpPost]
        public IActionResult Edit(int id, Instruction edited)
        {
            if (id != edited.Id) return NotFound();
            Instruction? instruction = _context.Instructions.FirstOrDefault(s => s.Id == id);
            if (instruction is null) return NotFound();
            bool duplicate = _context.Instructions.Any(s => s.QualityDetails == edited.QualityDetails && s.Lining == edited.Lining && s.Only == edited.Only && s.Clean == edited.Clean &&
            instruction.QualityDetails != edited.QualityDetails && instruction.Lining != edited.Lining && instruction.Only != edited.Only && instruction.Clean != edited.Clean
            );
            if (duplicate)
            {
                ModelState.AddModelError("Name", "This is now available");
                return View();
            }
            instruction.QualityDetails = edited.QualityDetails;
            instruction.Lining = edited.Lining;
            instruction.Only = edited.Only;
            instruction.Clean = edited.Clean;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            Instruction? instruction = _context.Instructions.FirstOrDefault(s => s.Id == id);
            return instruction is null ? BadRequest() : View(instruction);
        }



        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Instruction? instruction = _context.Instructions.FirstOrDefault(s => s.Id == id);

            if (instruction is null) return NotFound();
            return View(instruction);
        }

        [HttpPost]
        public IActionResult Delete(int id, Instruction deleted)
        {
            if (id != deleted.Id) return NotFound();
            Instruction? instruction = _context.Instructions.FirstOrDefault(s => s.Id == id);
            if (instruction is null) return NotFound();
            _context.Instructions.Remove(instruction);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
