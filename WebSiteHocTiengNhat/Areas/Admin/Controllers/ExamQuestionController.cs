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
    //[Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ExamQuestionController: Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ICategoryQuestionRepository _categoryQuestionRepository;
        private readonly ApplicationDbContext _context;
        
        public ExamQuestionController(IQuestionRepository questionRepository, ICategoryRepository categoryRepository,
        IExercisesRepository exercisesRepository, ICategoryQuestionRepository categoryQuestionRepository, ApplicationDbContext applicationDbContext)
        {
            _questionRepository = questionRepository;
            _exercisesRepository = exercisesRepository;
            _categoryRepository = categoryRepository;
            _categoryQuestionRepository = categoryQuestionRepository;
            _context= applicationDbContext;
        }


        public async Task<IActionResult> _ExamQuestionListPartial(int examid)
        {
            var examquestions = _context.ExamQuestions.ToList();
            var filterquestion=examquestions.Where(n=>n.ExamId == examid);
            return PartialView("_ExamQuestionListPartial", filterquestion);   
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

            var fileName = Guid.NewGuid() + Path.GetExtension(audio.FileName); // Sử dụng Guid để tránh trùng tên file
            var savePath = Path.Combine(directoryPath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await audio.CopyToAsync(fileStream);
            }

            return "/audio/" + fileName;
        }
       

        public async Task<IActionResult> Create(int examId)
        {
            var examquestion = new ExamQuestion { ExamId= examId};
            ViewBag.CorrectAnswer = new SelectList(new List<string> {"A", "B", "C", "D" });
            var categoryquestion = _context.CategoryQuestions.ToList();
            ViewBag.CategoryQuestion = new SelectList(categoryquestion, "CategoryQuestionId", "CategoryQuestionName");
            return View(examquestion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExamQuestion? eq, IFormFile? audioFile)
        {
            var categoryquestion = _context.CategoryQuestions.FirstOrDefault(n => n.CategoryQuestionId == eq.CategoryQuestionId);
            if (categoryquestion.IsListening)
            {
                if (audioFile != null)
                {
                    try
                    {
                        var audioPath = await SaveAudio(audioFile);
                        eq.Link = audioPath;
                        _context.ExamQuestions.Add(eq);
                        _context.SaveChanges();
                        return RedirectToAction("Detail", "Exam", new { Id = eq.ExamId });

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error saving exam question: {ex.Message}");
                    }
                }
                ModelState.AddModelError("", $"Listening question need audio file");
            }
            else { 
                try
                {
                    _context.ExamQuestions.Add(eq);
                    _context.SaveChanges();
                    return RedirectToAction("Detail", "Exam", new { Id = eq.ExamId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error saving exam question: {ex.Message}");
                }
            }
            var categoryquestionlist = _context.CategoryQuestions.ToList();
            ViewBag.CategoryQuestion = new SelectList(categoryquestionlist, "CategoryQuestionId", "CategoryQuestionName");
            ViewBag.CorrectAnswer = new SelectList(new List<string> { "A", "B", "C", "D" });
            return View(eq);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eq= _context.ExamQuestions.FirstOrDefault(n=>n.Id==id);
            if (eq == null)
            {
                return NotFound();
            }
            _context.ExamQuestions.Remove(eq);
            _context.SaveChanges();
            return RedirectToAction("Detail", "Exam", new { Id = eq.ExamId});
        }
    }
}
