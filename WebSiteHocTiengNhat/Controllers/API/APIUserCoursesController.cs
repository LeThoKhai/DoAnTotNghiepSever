using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repositories;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserCoursesController : ControllerBase
//    {
//        private readonly IUserCourseRepository _userCourseRepository;
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly ICoursesRepository _coursesRepository;
//        private readonly IScoreTableRepository _scoreTableRepository;
//        private readonly ICategoryRepository _categoryRepository;

//        public UserCoursesController(IUserCourseRepository userCourseRepository, UserManager<IdentityUser> userManager,
//            ICoursesRepository coursesRepository, IScoreTableRepository scoreTableRepository, ICategoryRepository categoryRepository)
//        {
//            _userCourseRepository = userCourseRepository;
//            _userManager = userManager;
//            _coursesRepository = coursesRepository;
//            _scoreTableRepository = scoreTableRepository;
//            _categoryRepository = categoryRepository;
//        }

//        // Kiểm tra nếu user đã tham gia vào khóa học hay chưa
//        [HttpGet("check-user-in-course/{courseId}")]
//        public async Task<IActionResult> CheckUserInCourse(int courseId)
//        {
//            var userId = User.FindFirst("userId")?.Value;
//            var userCourse = await _userCourseRepository.GetUserCoursesByCourseIdAndUserIdAsync(courseId, userId);

//            if (userCourse.Any())
//            {
//                return Ok(true); 
//            }
//            return Ok(false); 
//        }
//        [HttpGet("get-users-by-course/{courseId}")]
//        public async Task<IActionResult> GetUsersByCourse(int courseId)
//        {
//            var userCourses = await _userCourseRepository.GetUserCoursesByCourseIdAsync(courseId);
//            if (userCourses == null || !userCourses.Any())
//            {
//                return NotFound("No users found in this course.");
//            }

//            var users = userCourses.Select(uc => new
//            {
//                uc.UserId,
//                UserName = uc.User.UserName,
//                Email = uc.User.Email,
//                ExpirationTime = uc.ExpirationTime,
//                progress = uc.progress
//            }).ToList();

//            return Ok(users); // Trả về danh sách user trong khóa học
//        }

//        // Lấy thông tin user trong khóa học theo userId và courseId
//        [HttpGet("get-user-in-course/{courseId}")]
//        public async Task<IActionResult> GetUserInCourse(int courseId)
//        {
//            var userId = User.FindFirst("userId")?.Value;
//            var userCourse = await _userCourseRepository.GetUserCoursesByCourseIdAndUserIdAsync(courseId, userId);

//            if (userCourse == null || !userCourse.Any())
//            {
//                return NotFound("User not found in this course.");
//            }

//            var user = userCourse.Select(uc => new
//            {
//                uc.UserId,
//                UserName = uc.User.UserName,
//                Email = uc.User.Email,
//                ExpirationTime = uc.ExpirationTime,
//                progress = uc.progress
//            }).FirstOrDefault();

//            return Ok(user); // Trả về thông tin user trong khóa học
//        }
//        [HttpGet("get-courses-by-user")]
//        public async Task<IActionResult> GetCoursesByUserId()
//        {
//            var userId = User.FindFirst("userId")?.Value;
//            var userCourses = await _userCourseRepository.GetUserCoursesByUserIdAsync(userId);

//            if (userCourses == null || !userCourses.Any())
//            {
//                return NotFound("This user is not enrolled in any courses.");
//            }

//            var courses = userCourses.Select(uc => new
//            {
//                uc.CourseId,
//                CourseName = uc.Course.CourseName,
//                CourseDescription = uc.Course.Content,
//                ExpirationTime = uc.ExpirationTime,
//                progress = uc.progress


//            }).ToList();

//            return Ok(courses); 
//        }
//        [HttpGet("get_score_table")]
//        public async Task<IActionResult> GetScoreTable()
//        {
//            var userid = User.FindFirst("userId")?.Value;
//            var categories = await _categoryRepository.GetAllAsync();
//            var result = new List<AverageScore>();

//            foreach (var category in categories)
//            {
//                // Kiểm tra xem có điểm nào cho userId và categoryId này không
//                bool hasScore = await _scoreTableRepository.CheckCategoryInScoreTable(userid, category.CategoryId);

//                if (hasScore) // Chỉ tính toán nếu có điểm
//                {
//                    double averageScore = await _scoreTableRepository.GetAverageScoreByCategoryAsync(userid, category.CategoryId);

//                    if (!double.IsNaN(averageScore))
//                    {
//                        result.Add(new AverageScore
//                        {
//                            Score = averageScore,
//                            CategoryName = category.CategoryName
//                        });
//                    }
//                }
//            }

//            return Ok(result);
//        }


//        public class AverageScore
//        {
//            public double Score { get; set; }
//            public string? CategoryName { get; set; }
//        }
//    }
//
}
