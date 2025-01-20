using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Repository;
using System.Threading.Tasks;

namespace WebSiteHocTiengNhat.ViewComponents
{
    public class ListFlashCardViewComponent : ViewComponent
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IFlashCardRepository _flashCardRepository;

        public ListFlashCardViewComponent(ILessonRepository lessonRepository,
            IFlashCardRepository flashCardRepository)
        {
            _lessonRepository = lessonRepository;
            _flashCardRepository = flashCardRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int lessonId)
        {
            var flashcards = await _flashCardRepository.GetByLessonIdAsync(lessonId);
            return View(flashcards);
        }
    }
}
