using Azure.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Diagnostics;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repositories;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICoursesRepository _coursesRepository;
        private readonly IUserCourseRepository _userCourseRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(UserManager<IdentityUser> userManager, ICoursesRepository coursesRepository,IUserCourseRepository userCourseRepository)
        {
            _coursesRepository = coursesRepository;
            _userCourseRepository = userCourseRepository;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var courses = await _coursesRepository.GetAllAsync();
            courses=courses.Where(n=>n.Status==true).ToList();

            return View(courses);
        }


    }
}
