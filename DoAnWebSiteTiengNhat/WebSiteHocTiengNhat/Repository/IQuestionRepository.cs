using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question> GetByIdAsync(int id);
        Task <List<Question>> GetByExerciseId(int? exerciseId);
        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(int? id);
    }
}
