using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public interface ICategoryQuestionRepository
    {
        Task<IEnumerable<CategoryQuestion>> GetAllAsync();
        Task<CategoryQuestion> GetByIdAsync(int? id);
        Task AddAsync(CategoryQuestion categoryquestion);
        Task UpdateAsync(CategoryQuestion categoryquestion);
        Task DeleteAsync(int id);
        Task<List<Question>> GetByCategoryQuestionId(int? categoryquestionId);


    }
}
