using Microsoft.AspNetCore.Mvc.Rendering;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class LessonsViewModel
    {
        public string? CourseName { get; set; }
        public int? CourseId {  get; set; }
        public int? LessonId { get; set; }
        public List<Lesson>? Lessons { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<FlashCard>? flashCards { get; set; }
    }
}
