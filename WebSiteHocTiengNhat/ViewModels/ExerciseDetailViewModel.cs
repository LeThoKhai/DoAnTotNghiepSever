using WebSiteHocTiengNhat.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class ExerciseDetailViewModel
    {
        public int ExerciseId {  get; set; }
        public string ExerciseName { get; set; }
        public string? Content { get; set; }
        public int CourseId {  get; set; }
        public List<Question> questions { get; set; }
    }
}
