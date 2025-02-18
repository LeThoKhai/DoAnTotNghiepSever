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
    public class SpeakingQuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpeakingQuestionController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var list = _context.SpeakingQuestions.ToList();
            return View(list);
        }
        private async Task<string> SaveAudio(IFormFile audio)
        {
            if (audio == null || audio.Length == 0)
            {
                throw new ArgumentException("Audio file is invalid.");
            }
            var directoryPath = Path.Combine("wwwroot/audio");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(audio.FileName);
            var savePath = Path.Combine(directoryPath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await audio.CopyToAsync(fileStream);
            }

            return "/audio/" + fileName;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpeakingQuestion SpeakingQuestion, IFormFile? audioFile)
        {
            try
            {
                if (audioFile != null)
                {
                    var audioPath = await SaveAudio(audioFile);
                    SpeakingQuestion.Link = audioPath;
                }
                await _context.AddAsync(SpeakingQuestion);
                _context.SaveChanges();
                return RedirectToAction("Index", "SpeakingQuestion");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving audio: {ex.Message}");
            }
            ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            return View(SpeakingQuestion);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SpeakingQuestions.FirstOrDefaultAsync(n => n.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            _context.SpeakingQuestions.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index", "SpeakingQuestion");
        }
    }
}
