using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public interface ILessonRepository
    {
        Task<IEnumerable<Lesson>> GetAllAsync();
        Task<Lesson> GetByIdAsync(int? id);
        Task<Lesson> GetByCourseId(int courseId);
        Task<List<Lesson>> GetByCourseIdAndCategoryIdAsync(int courseId, int categoryId);
        Task AddAsync(Lesson lesson);
        Task UpdateAsync(Lesson lesson);
        Task DeleteAsync(int id);
        Task<IEnumerable<Lesson>> GetListLessonByCourseId(int courseId);

    }
}
