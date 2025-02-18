using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.ViewModels;

namespace WebSiteHocTiengNhat.Controllers
{
    public class UserExercisesController : Controller
    {
        private readonly ICoursesRepository _coursesRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IQuestionRepository _questionRepository;
        public UserExercisesController(ICategoryRepository categoryRepository, ICoursesRepository coursesRepository,
            IExercisesRepository exercisesRepository, IQuestionRepository questionRepository)
        {
            _categoryRepository = categoryRepository;
            _coursesRepository = coursesRepository;
            _exercisesRepository = exercisesRepository;
            _questionRepository = questionRepository;
        }
        public async Task<IActionResult> Index(int courseId, int? categoryId, int? exerciseId)
        {
            ViewBag.CourseId = courseId;
            var viewmodel = new UserExerciseViewModel
            {
                course = await _coursesRepository.GetByIdAsync(courseId),
                exercise = await _exercisesRepository.GetByCourseIdAsync(courseId),
               
            };

            if (exerciseId.HasValue && categoryId.HasValue)
            {
                ViewBag.categoryId = categoryId;
                var exercises = await _exercisesRepository.GetAllAsync();
                viewmodel.exercise = exercises.FirstOrDefault(n => n.ExerciseId == exerciseId && n.CategoryId == categoryId && n.CourseId == courseId);

                if (viewmodel.exercise == null)
                {
                    return NotFound("Bài học không tồn tại.");
                }
                viewmodel.category = await _categoryRepository.GetByIdAsync(categoryId);
                viewmodel.questions = await _questionRepository.GetByExerciseId(exerciseId);
            }

            if (viewmodel.exercise == null)
            {
                return NotFound("Bài tập bị rỗng");
            }

            return View(viewmodel);
        }
        [HttpPost]
        public async Task<IActionResult> Submit(int courseId, int exerciseId, int categoryId, int[] questionIds, string[] selectedAnswers)
        {
            int correctCount = 0;
            ViewBag.CourseId = courseId;
            // Loop through each question and check the answer
            for (int i = 0; i < questionIds.Length; i++)
            {
                var question = await _questionRepository.GetByIdAsync(questionIds[i]);
                if (question != null && question.CorrectAnswer == selectedAnswers[i])
                {
                    correctCount++;
                }
            }

            // Calculate the score
            int totalQuestions = questionIds.Length;
            double score = (double)correctCount / totalQuestions * 100;

            // Create a view model with the score
            var viewmodel = new UserExerciseViewModel
            {
                course = await _coursesRepository.GetByIdAsync(courseId),
                exercise = await _exercisesRepository.GetByIdAsync(exerciseId),
                questions = await _questionRepository.GetByExerciseId(exerciseId),
                category=await _categoryRepository.GetByIdAsync(categoryId),
                Score = score,
                Answer=true,        
            };
            ViewBag.courseId=courseId;
            ViewBag.excerciseId=exerciseId;
            ViewBag.categoryId=categoryId;
            return View("Index", viewmodel);
        }

    }
}
