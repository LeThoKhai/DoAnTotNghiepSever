using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class Course_List_Category_ExerciseViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<Category> categories { get; set; }
        public List<Exercise> exercises { get; set; }
    }
}
