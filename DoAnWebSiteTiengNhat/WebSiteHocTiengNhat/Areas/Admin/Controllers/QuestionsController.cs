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
    public class QuestionsController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ICategoryQuestionRepository _categoryQuestionRepository;
        private readonly ApplicationDbContext _context;
        
        public QuestionsController(IQuestionRepository questionRepository, ICategoryRepository categoryRepository,
        IExercisesRepository exercisesRepository, ICategoryQuestionRepository categoryQuestionRepository, ApplicationDbContext applicationDbContext)
        {
            _questionRepository = questionRepository;
            _exercisesRepository = exercisesRepository;
            _categoryRepository = categoryRepository;
            _categoryQuestionRepository = categoryQuestionRepository;
            _context= applicationDbContext;
        }


        public async Task<IActionResult> _QuestionListPartial(int exerciseId)
        {
            var questions = await _questionRepository.GetAllAsync();
            var filterquestion=questions.Where(n=>n.ExerciseId == exerciseId);
            return PartialView("_QuestionListPartial", filterquestion);   
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


        public async Task<IActionResult> Create(int exerciseId)
        {
            // Khởi tạo một model mới cho view Create
            var question = new Question { ExerciseId= exerciseId};
            var categoryquestion = await _categoryQuestionRepository.GetAllAsync();
            ViewBag.CategoryQuestion = new SelectList(categoryquestion, "CategoryQuestionId", "CategoryQuestionName");
            var questiontype =await _context.QuestionTypes.ToListAsync();
            ViewBag.QuestionType = new SelectList(questiontype,"QuestionTypeId","QuestionTypeName");
            ViewBag.CorrectAnswer = new SelectList(new List<string> { "Empty","A", "B", "C", "D" });
            ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            ViewBag.ExerciseId= exerciseId;
            // Trả về view Create với model đã khởi tạo
            return View(question);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Question? question, IFormFile? audioFile)
        {
            if (ModelState.IsValid)
            {
                if (question.QuestionTypeId != "QT2" && question.CorrectAnswerString == "Empty")
                {
                    if((question.QuestionTypeId == "QT8" || question.QuestionTypeId == "QT9") && audioFile==null)
                    {
                        ModelState.AddModelError("", "Thiếu file âm thanh hoặc hình ảnh và chuỗi đáp án không được để trống.");
                        var cq1 = await _categoryQuestionRepository.GetAllAsync();
                        ViewBag.CategoryQuestion = new SelectList(cq1, "CategoryQuestionId", "CategoryQuestionName");
                        var qt1 = await _context.QuestionTypes.ToListAsync();
                        ViewBag.QuestionType = new SelectList(qt1, "QuestionTypeId", "QuestionTypeName");
                        ViewBag.CorrectAnswer = new SelectList(new List<string> { "A", "B", "C", "D" });
                        ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
                        return View(question);
                    }
                    ModelState.AddModelError("", "Thiếu thông tin cho chuỗi đáp án.");
                    var cq = await _categoryQuestionRepository.GetAllAsync();
                    ViewBag.CategoryQuestion = new SelectList(cq, "CategoryQuestionId", "CategoryQuestionName");
                    var qt = await _context.QuestionTypes.ToListAsync();
                    ViewBag.QuestionType = new SelectList(qt, "QuestionTypeId", "QuestionTypeName");
                    ViewBag.CorrectAnswer = new SelectList(new List<string> { "A", "B", "C", "D" });
                    ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
                    return View(question);
                }
                try
                {
                    if (audioFile != null)
                    {
                        var audioPath = await SaveAudio(audioFile);
                        question.Link = audioPath;
                    }
                    await _questionRepository.AddAsync(question);
                    return RedirectToAction("Detail", "Exercises", new { Id = question.ExerciseId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error saving audio: {ex.Message}");
                }
            }

            // Nếu lỗi, trả về view với dữ liệu đã nhập
            var categoryquestion = await _categoryQuestionRepository.GetAllAsync();
            ViewBag.CategoryQuestion = new SelectList(categoryquestion, "CategoryQuestionId", "CategoryQuestionName");
            var questiontype = await _context.QuestionTypes.ToListAsync();
            ViewBag.QuestionType = new SelectList(questiontype, "QuestionTypeId", "QuestionTypeName");
            ViewBag.CorrectAnswer = new SelectList(new List<string> { "Empty","A", "B", "C", "D" });
            ViewBag.Level = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Lấy câu hỏi dựa trên id
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null)
            {
                // Nếu câu hỏi không tồn tại, trả về lỗi
                return NotFound();
            }

            // Tiến hành xóa câu hỏi
            await _questionRepository.DeleteAsync(id);

            // Điều hướng về trang chi tiết bài tập
            return RedirectToAction("Detail", "Exercises", new { Id = question.ExerciseId });
        }
    }
}
