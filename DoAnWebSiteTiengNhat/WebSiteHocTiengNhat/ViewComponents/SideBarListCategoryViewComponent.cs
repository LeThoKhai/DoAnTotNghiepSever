using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.ViewModels;

namespace WebSiteHocTiengNhat.ViewComponents
{
    public class SideBarListCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ICoursesRepository _coursesRepository;

        public SideBarListCategoryViewComponent(ICoursesRepository coursesRepository, ICategoryRepository categoryRepository,ILessonRepository lessonRepository)
        {
            _categoryRepository = categoryRepository;
            _lessonRepository = lessonRepository;
            _coursesRepository = coursesRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy courseId từ Session và kiểm tra null
            int courseId = ViewBag.courseId;
            // _httpContextAccessor.HttpContext.Session.GetInt32("CourseId");
            //if (!courseId.HasValue)
            //{
            //    return Content("CourseId không được tìm thấy trong session.");
            //}
            // Lấy danh sách category
            var categories = await _categoryRepository.GetAllAsync();
            var course= await _coursesRepository.GetByIdAsync(courseId);
            // Lấy danh sách bài học dựa trên courseId
            var lessons = new List<Lesson>();
            foreach (var category in categories)
            {
                // Lấy bài học cho từng category và courseId
                var categoryLessons = await _lessonRepository.GetByCourseIdAndCategoryIdAsync(courseId, category.CategoryId);
                lessons.AddRange(categoryLessons);
            }
            // Tạo ViewModel
            var viewModel = new Course_List_Category_LessonViewModel
            {
                CourseId = courseId,
                CourseName =  course.CourseName, // Bạn cần lấy tên khóa học từ đâu đó hoặc từ một phương thức khác
                categories = categories.ToList(),
                lessons = lessons
            };
            // Trả về ViewComponentResult với ViewModel
            return View(viewModel);
        }

    }
}
