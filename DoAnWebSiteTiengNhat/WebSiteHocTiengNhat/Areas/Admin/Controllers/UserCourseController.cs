using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repositories;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.ViewModels;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class UserCourseController : Controller
    {
        private readonly IUserCourseRepository _userCourseRepository;
        private readonly ICoursesRepository _coursesRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public UserCourseController(UserManager<IdentityUser> userManager
            , ICoursesRepository coursesRepository,IUserCourseRepository userCourseRepository)
        {
            _userCourseRepository = userCourseRepository;
            _coursesRepository = coursesRepository;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var courses = await _coursesRepository.GetAllAsync();
            return View(courses);
        }
        public async Task<IActionResult> AddUserInCourse(int courseId, string? search)
        {
            // Lấy danh sách các UserCourses của khóa học hiện tại
            var userInCourseIds = (await _userCourseRepository.GetUserCoursesByCourseIdAsync(courseId))
                .Select(uc => uc.UserId).ToList();

            // Lấy tất cả người dùng và lọc ra những người chưa tham gia khóa học
            var users = await _userManager.Users
                .Where(u => !userInCourseIds.Contains(u.Id))
                .ToListAsync();

            // Nếu có chuỗi tìm kiếm, lọc tiếp theo chuỗi tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.CourseId = courseId;
            return View(users); // Trả về danh sách người dùng chưa tham gia khóa học
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserInCourse(string userId, int courseId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "UserId cannot be null or empty.");
                return RedirectToAction("AddUserInCourse", new { courseId }); // Trả về trang hiện tại với thông báo lỗi
            }
            // Tạo mới đối tượng UserCourse
            var userCourse = new UserCourse
            {
                CourseId = courseId,
                UserId = userId,
                ExpirationTime = DateTime.UtcNow.AddMonths(3) // Thời gian hết hạn
            };
            // Thêm người dùng vào khóa học
            await _userCourseRepository.AddUserToCourseAsync(userCourse);
            // Điều hướng tới danh sách người dùng trong khóa học
            return RedirectToAction("GetUserCoursesByCourseId", "UserCourse", new { courseId });
        }


        [HttpGet]
        public async Task<IActionResult> GetUserCoursesByUserId(string userId)
        {
            var userCourses = await _userCourseRepository.GetUserCoursesByUserIdAsync(userId);
            return View(userCourses);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCoursesByCourseId(int courseId, string? search)
        {
            // Lấy tất cả các UserCourses dựa trên CourseId
            var userCourses = await _userCourseRepository.GetUserCoursesByCourseIdAsync(courseId);
            ViewBag.CourseId = courseId.ToString();
            //TempData["courseId"] = courseId;
            //COURSEID = courseId;
            if (!string.IsNullOrEmpty(search))
            {
                // Lấy tất cả người dùng từ cơ sở dữ liệu
                var users = await _userManager.Users.ToListAsync();

                // Lọc người dùng dựa trên chuỗi tìm kiếm (không phân biệt chữ hoa chữ thường)
                var filteredUsers = users
                    .Where(u => u.UserName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                if (!filteredUsers.Any())
                {
                    return View(new List<UserManager>());
                }

                var userIds = filteredUsers.Select(u => u.Id).ToList();

                // Lọc các UserCourses theo userIds
                userCourses = userCourses.Where(uc => userIds.Contains(uc.UserId)).ToList();
            }

            var course = await _coursesRepository.GetByIdAsync(courseId);

            // Tạo danh sách UserCourseViewModel
            var userCourseViewModels = userCourses.Select(userCourse => new UserManager
            {
                UserId = userCourse.UserId,
                UserName = userCourse.User.UserName,
                UserEmail = userCourse.User.Email,
                CourseId = userCourse.CourseId,
                CourseName = course.CourseName,
                ExpirationTime = userCourse.ExpirationTime,
            }).ToList();
            return View(userCourseViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserCourses()
        {
            var userCourses = await _userCourseRepository.GetAllUserCoursesAsync();
            return View(userCourses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserCourse(int courseId, string userId)
        {
            if (courseId <= 0 || string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "CourseId or UserId is invalid.");
                return RedirectToAction("Index");
            }

            // Get the list of UserCourses that match the courseId and userId
            var userCourses = await _userCourseRepository.GetUserCoursesByCourseIdAndUserIdAsync(courseId, userId);

            if (!userCourses.Any())
            {
                ModelState.AddModelError("", "The UserCourse does not exist.");
                return RedirectToAction("Index");
            }

            // Loop through and delete each UserCourse
            foreach (var userCourse in userCourses)
            {
                await _userCourseRepository.DeleteUserCourseAsync(userCourse);
            }

            TempData["SuccessMessage"] = "User successfully removed from the course.";
            return RedirectToAction("GetUserCoursesByCourseId", new { courseId });
        }

    }
}
