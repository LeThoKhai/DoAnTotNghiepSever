using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public interface IFlashCardRepository
    {
        Task<IEnumerable<FlashCard>> GetAllAsync();
        Task<IEnumerable<FlashCard>> GetByLessonIdAsync(int LessonId);
        Task<FlashCard> GetByIdAsync(int id);
        Task AddAsync(FlashCard flashcard);
        Task UpdateAsync(FlashCard flashcard);
        Task DeleteAsync(int id);
    }
}
