using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReadingQuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReadingQuestionController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var list = _context.ReadingQuestions.ToList();
            return View(list);
        }
        private async Task<string> Saveimg(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("Img file is invalid.");
            }
            var directoryPath = Path.Combine("wwwroot/images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var savePath = Path.Combine(directoryPath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            ViewBag.CorrectAnswer = new SelectList(new List<string> { "A", "B", "C", "D" });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReadingQuestion ReadingQuestion, IFormFile? imgFile)
        {
            try
            {
                if (imgFile != null)
                {
                    var imgPath = await Saveimg(imgFile);
                    ReadingQuestion.Link = imgPath;
                }
                await _context.AddAsync(ReadingQuestion);
                _context.SaveChanges();
                return RedirectToAction("Index", "ReadingQuestion");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving img: {ex.Message}");
            }
            ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            ViewBag.CorrectAnswer = new SelectList(new List<string> { "A", "B", "C", "D" });
            return View(ReadingQuestion);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ReadingQuestions.FirstOrDefaultAsync(n => n.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            _context.ReadingQuestions.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index", "ReadingQuestion");
        }
    }
}
