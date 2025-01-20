using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.ViewModels;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{

    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class FlashCardsController : Controller
    {
        private readonly ICoursesRepository _coursesRepository;
        private readonly IFlashCardRepository _flashCardRepostitory;
        private readonly ILessonRepository _lessonRepository;

        public FlashCardsController(ILessonRepository lessonRepository,ICoursesRepository coursesRepository, IFlashCardRepository flashCardRepostitory)
        {
            _coursesRepository = coursesRepository;
            _lessonRepository = lessonRepository;
            _flashCardRepostitory = flashCardRepostitory;
        }
        public async Task<IEnumerable<FlashCard>> GetFlashCardByLessonId(int lessonId)
        {
            var flashcards = await _flashCardRepostitory.GetAllAsync();
            flashcards = flashcards.Where(l =>l.LessonId  == lessonId);
            return flashcards;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _coursesRepository.GetAllAsync();
            return View(courses);
        }
        public async Task<IActionResult> LessonList(int courseId, string? search)
        {
            TempData["CourseId"] = courseId;
            // Lấy danh sách tất cả các bài học
            var lessons = await _lessonRepository.GetAllAsync();
            // Lọc bài học theo courseId
            var filteredLessons = lessons.Where(l => l.CourseId == courseId);
            if (!string.IsNullOrEmpty(search))
            {
                filteredLessons = filteredLessons.Where(n => n.LessonName.Contains(search));               
            }
            // Lấy thông tin khóa học
            var course = await _coursesRepository.GetByIdAsync(courseId);
            var courseName = course?.CourseName;

            // Tạo ViewModel và truyền dữ liệu vào
            var viewModel = new LessonsViewModel
            {
                CourseName = courseName,
                CourseId = courseId,
                Lessons = filteredLessons.ToList(),
            };
            return View(viewModel);
        }
        public async Task<IActionResult> FlashCardList(int lessonId, string? search)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId);

            // Lấy giá trị courseId từ TempData
            var courseId = TempData["CourseId"] != null ? Convert.ToInt32(TempData["CourseId"]) : 0;

            var flashcards = await GetFlashCardByLessonId(lessonId);
            if (!string.IsNullOrEmpty(search))
            {
                flashcards = flashcards.Where(n => n.CardName.Contains(search));
            }
            var viewModel = new FlashCard_LessonViewModel
            {
                CourseId = courseId,
                LessonId = lesson.LessonId,
                LessonName = lesson.LessonName,
                FlashCards = flashcards.ToList(),
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create(int lessonId)
        {
            // Khởi tạo một model mới cho view Create
            var flashCard = new FlashCard
            {
                LessonId = lessonId // Gán giá trị lessonId vào thuộc tính LessonId của model
            };

            return View(flashCard); // Truyền model vào view
        }

        [HttpPost]
        public async Task<IActionResult> Create(FlashCard flashcard)
        {
            if (ModelState.IsValid)
            {
                await _flashCardRepostitory.AddAsync(flashcard);
                return RedirectToAction(nameof(FlashCardList), new { lessonId = flashcard.LessonId});
            }
            return View(flashcard);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var flashCard = await _flashCardRepostitory.GetByIdAsync(id);
            if (flashCard == null)
            {
                return NotFound();
            }
            return View(flashCard);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id, FlashCard card)
        {
            var flashCard = await _flashCardRepostitory.GetByIdAsync(id);
            if (flashCard == null)
            {
                return NotFound();
            }
            int lessonId = flashCard.LessonId;
            await _flashCardRepostitory.DeleteAsync(id);
            return RedirectToAction(nameof(FlashCardList), new { LessonId= lessonId});
        }
    }
}
