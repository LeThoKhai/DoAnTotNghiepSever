using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.ViewComponents
{
    public class SideBarListCourseViewComponent : ViewComponent
    {
        private readonly ICoursesRepository _coursesRepository;
        public SideBarListCourseViewComponent(ICoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var courses = await _coursesRepository.GetAllAsync();
            courses = courses.Where(n => n.Status == true).ToList();
            return View(courses);
        }
    }
}
