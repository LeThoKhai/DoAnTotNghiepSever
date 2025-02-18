using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.ViewComponents
{
    public class ListLesson:ViewComponent
    {
        private readonly ILessonRepository _lessonRepository;
        public ListLesson(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(int lessonId,int categoryId)
        {
            var lesson = await _lessonRepository.GetAllAsync();
            lesson = lesson.Where(n => n.LessonId == lessonId && n.CategoryId==categoryId).ToList();
            return View(lesson);
        }
    }
}
