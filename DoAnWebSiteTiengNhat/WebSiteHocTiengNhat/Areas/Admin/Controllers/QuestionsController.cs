using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public QuestionsController(IQuestionRepository questionRepository, ICategoryRepository categoryRepository,
        IExercisesRepository exercisesRepository, ICategoryQuestionRepository categoryQuestionRepository)
        {
            _questionRepository = questionRepository;
            _exercisesRepository = exercisesRepository;
            _categoryRepository = categoryRepository;
            _categoryQuestionRepository = categoryQuestionRepository;
        }

        
        public async Task<IActionResult> _QuestionListPartial(int exerciseId)
        {
            var questions = await _questionRepository.GetAllAsync();
            var filterquestion=questions.Where(n=>n.ExerciseId == exerciseId);
            return PartialView("_QuestionListPartial", filterquestion);   
        }


        public async Task<IActionResult> Create(int exerciseId)
        {
            // Khởi tạo một model mới cho view Create
            var question = new Question { ExerciseId= exerciseId};
            var categoryquestion = await _categoryQuestionRepository.GetAllAsync();
            ViewBag.CategoryQuestion = new SelectList(categoryquestion, "CategoryQuestionId", "CategoryQuestionName");
            ViewBag.CorrectAnswer = new SelectList(new List<string> { "A", "B", "C", "D" });
            // Trả về view Create với model đã khởi tạo
            return View(question);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Question question)
        {
            if (ModelState.IsValid)
            {
                await _questionRepository.AddAsync(question);
                return RedirectToAction("Detail", "Exercises", new { Id = question.ExerciseId });
            }
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
