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
    public class WritingQuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WritingQuestionController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var list = _context.WritingQuestions.ToList();
            return View(list);
        }
        private async Task<string> Saveimg(IFormFile img)
        {
            if (img == null || img.Length == 0)
            {
                throw new ArgumentException("img file is invalid.");
            }
            var directoryPath = Path.Combine("wwwroot/images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(img.FileName);
            var savePath = Path.Combine(directoryPath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await img.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WritingQuestion WritingQuestion, IFormFile? imgFile)
        {
            try
            {
                if (imgFile != null)
                {
                    var imgPath = await Saveimg(imgFile);
                    WritingQuestion.Link = imgPath;
                }
                await _context.AddAsync(WritingQuestion);
                _context.SaveChanges();
                return RedirectToAction("Index", "WritingQuestion");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving img: {ex.Message}");
            }
            ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            return View(WritingQuestion);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.WritingQuestions.FirstOrDefaultAsync(n => n.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            _context.WritingQuestions.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index", "WritingQuestion");
        }
    }
}
