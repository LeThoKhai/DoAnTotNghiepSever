using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public interface IExercisesRepository
    {
        Task<IEnumerable<Exercise>> GetAllAsync();
        Task<Exercise> GetByIdAsync(int id);
        Task<Exercise> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<Exercise>> GetListByCourseIdAsync(int courseId);
        Task<List<Exercise>> GetByCourseIdAndCategoryIdAsync(int courseId, int categoryId);
        Task AddAsync(Exercise exercise);
        Task UpdateAsync(Exercise exercise);
        Task DeleteAsync(int id);
        Task<List<Exercise>> GetAllByCourseIdAsync(int courseId);
        Task<int> CountByCourseIdAsync(int CourseId);
    }
}
