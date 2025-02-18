using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class Course_LS_EX_FL_ViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<Exercise> exercise { get; set; }
        public List<Question> question { get; set; }
        public List<Lesson> lessons { get; set; }
    }
}
