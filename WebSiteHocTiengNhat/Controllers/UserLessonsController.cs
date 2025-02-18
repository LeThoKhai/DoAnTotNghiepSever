using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WebSiteHocTiengNhat.Repositories;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.Models;
namespace WebSiteHocTiengNhat.Controllers
{
    public class UserLessonsController : Controller
    {
        private readonly ICoursesRepository _coursesRepository;
        private readonly IUserCourseRepository _userCourseRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public UserLessonsController(UserManager<IdentityUser> userManager, ICoursesRepository coursesRepository, IUserCourseRepository userCourseRepository,ILessonRepository lessonRepository)
        {
            _coursesRepository = coursesRepository;
            _userCourseRepository = userCourseRepository;
            _userManager = userManager;
            _lessonRepository = lessonRepository;
        }
        public async Task<IActionResult> Index(int courseId, int? lessonId, int? categoryId)
        {
            var course = await _coursesRepository.GetByIdAsync(courseId);
            ViewBag.CourseId = courseId;
            var lessons = await _lessonRepository.GetAllAsync();
            var lessonbyid = await _lessonRepository.GetByCourseId(courseId);
            if (lessonId.HasValue && categoryId.HasValue)
            {
                var lesson = lessons.FirstOrDefault(n => n.LessonId == lessonId && n.CategoryId == categoryId&&n.CourseId==courseId);
                if (lesson == null)
                {
                    return NotFound("Bài học không tồn tại.");
                }
                return View(lesson); // Trả về một bài học duy nhất
            }
            // Nếu không tìm thấy bài học, bạn có thể xử lý logic khác ở đây
            return View(lessonbyid); // Trả về một bài học duy nhất

        }
    }
}
