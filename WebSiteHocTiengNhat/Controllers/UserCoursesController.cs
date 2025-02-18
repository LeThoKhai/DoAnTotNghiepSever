using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repositories;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.Controllers
{
    public class UserCoursesController : Controller
    {
        private readonly ICoursesRepository _coursesRepository;
        private readonly IUserCourseRepository _userCourseRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public UserCoursesController(UserManager<IdentityUser> userManager, ICoursesRepository coursesRepository, IUserCourseRepository userCourseRepository)
        {
            _coursesRepository = coursesRepository;
            _userCourseRepository = userCourseRepository;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        //public IActionResult CourseDetail(int courseid, string userid)
        //{
        //    return View();
        //}
        public async Task<bool> CheckUserCourse(string userId, int courseId)
        {
            // Lấy danh sách các UserCourse theo courseId
            var userCourses = await _userCourseRepository.GetUserCoursesByCourseIdAsync(courseId);

            // Kiểm tra xem trong danh sách có phần tử nào có UserId trùng khớp với userId truyền vào không
            if (userCourses.Any(uc => uc.UserId == userId))
            {
                return true; // Nếu tìm thấy, trả về true
            }
            else
            {
                return false; // Nếu không tìm thấy, trả về false
            }
        }

        [HttpGet]
        public async Task<IActionResult> CourseDetail(int courseId)
        {
            var course=await _coursesRepository.GetByIdAsync(courseId);
            TempData["courseId"] = course.CourseId;
            return View(course);
        }
        [HttpGet]
        public async Task<IActionResult> Collection()
        {
            int courseId = (int)TempData["courseId"];
            var course = await _coursesRepository.GetByIdAsync(courseId);
            
            TempData.Keep("courseId");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            bool hasUserCourse = await CheckUserCourse(user.Id, courseId);
            if (hasUserCourse)
            {
                return View(course);
             
            }
            else
            {
                return View("Index");
            }
        }

        //public async Task<bool> CheckUserCourse(string userId, int courseId)
        //{
        //   var usercourse= await _userCourseRepository.GetUserCoursesByCourseIdAndUserIdAsync(courseId, userId);
        //    if (usercourse != null) { 
        //        return true; 
        //    }
        //    else { 
        //        return false; 
        //    }
        //}
        //public async Task<IActionResult> CourseButtonClick(int courseId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {               
        //        return RedirectToPage("/Account/Login", new { area = "Identity" });
        //    }
        //    bool hasUserCourse = await CheckUserCourse(user.Id, courseId);
        //    if (hasUserCourse)
        //    {

        //        return RedirectToAction("CourseDetail", "Courses", new { courseId = courseId, userId = user.Id });
        //    }
        //    else
        //    {              
        //        return View("Index");
        //    }
        //}
    }
}
