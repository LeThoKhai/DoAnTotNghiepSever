using Microsoft.AspNetCore.Mvc.Rendering;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class ExerciseViewModel
    {
        public string CourseName { get; set; }
        public int CourseId { get; set; }
        public List<Exercise> Exercises { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
