using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class Course_List_Category_LessonViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<Category> categories{ get; set; }
        public List<Lesson> lessons{ get; set; }
    }
}
