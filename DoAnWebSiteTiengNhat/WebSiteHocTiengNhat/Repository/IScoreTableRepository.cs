using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public interface IScoreTableRepository
    {
        Task<ScoreTable> GetByExerciseIdAndUserIdAsync(int exerciseId, string userId);
        Task AddAsync(ScoreTable scoreTable);
        Task UpdateAsync(ScoreTable scoreTable);
        Task<List<ScoreTable>> GetAllByUserIdAsync(string userId);
        Task<int> CountByUserIdAsync(string userId);
        Task<double> GetAverageScoreByCategoryAsync(string userId, int exerciseId, int categoryId);
        Task<bool> CheckCategoryInScoreTable(string userId, int categoryId);
        Task<double> GetAverageScoreByCategoryAsync(string userId, int categoryId);

    }
}
