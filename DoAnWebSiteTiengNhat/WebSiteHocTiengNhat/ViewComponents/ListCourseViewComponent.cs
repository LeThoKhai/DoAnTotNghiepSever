using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.ViewComponents
{
    public class ListCourseViewComponent : ViewComponent
    {
        private readonly ICoursesRepository _courseRepository;

        public ListCourseViewComponent(ICoursesRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count)
        {
            var courses = await _courseRepository.GetAllAsync();
            courses=courses.Where(n=>n.Status==true).ToList();
            return View(courses);
        }
    }
}
