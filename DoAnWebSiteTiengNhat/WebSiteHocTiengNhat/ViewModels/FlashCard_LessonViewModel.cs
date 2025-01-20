using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class FlashCard_LessonViewModel
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }

        public List<FlashCard>? FlashCards { get; set; }
        public int? CourseId{ get; set; }
    }
}
