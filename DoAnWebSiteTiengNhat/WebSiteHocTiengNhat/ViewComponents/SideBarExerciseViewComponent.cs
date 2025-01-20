using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.ViewModels;

namespace WebSiteHocTiengNhat.ViewComponents
{
    public class SideBarExerciseViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly ICoursesRepository _coursesRepository;

        public SideBarExerciseViewComponent(ICoursesRepository coursesRepository, ICategoryRepository categoryRepository,IExercisesRepository exercisesRepository)
        {
            _categoryRepository = categoryRepository;
            _coursesRepository = coursesRepository;
            _exercisesRepository = exercisesRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy courseId từ Session và kiểm tra null
            int courseId = ViewBag.courseId;
            // Lấy danh sách category
            var categories = await _categoryRepository.GetAllAsync();
            var course = await _coursesRepository.GetByIdAsync(courseId);
            // Lấy danh sách bài học dựa trên courseId
            var exercises = new List<Exercise>();
            foreach (var category in categories)
            {
                // Lấy bài học cho từng category và courseId
                var categoryExercise= await _exercisesRepository.GetByCourseIdAndCategoryIdAsync(courseId, category.CategoryId);
                exercises.AddRange(categoryExercise);
            }
            // Tạo ViewModel
            var viewModel = new Course_List_Category_ExerciseViewModel
            {
                CourseId = courseId,
                CourseName = course.CourseName,
                categories = categories.ToList(),
                exercises = exercises,
            };
            // Trả về ViewComponentResult với ViewModel
            return View(viewModel);
        }

    }
}
