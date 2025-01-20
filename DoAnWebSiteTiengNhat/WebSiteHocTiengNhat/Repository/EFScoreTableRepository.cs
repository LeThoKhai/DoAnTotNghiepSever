using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;
using Microsoft.EntityFrameworkCore;

namespace WebSiteHocTiengNhat.Repository
{
    public class EFScoreTableRepository:IScoreTableRepository
    {
        private readonly ApplicationDbContext _context;

        public EFScoreTableRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ScoreTable> GetByExerciseIdAndUserIdAsync(int exerciseId, string userId)
        {
            return await _context.ScoreTables
                .FirstOrDefaultAsync(s => s.ExerciseId == exerciseId && s.UserId == userId);
        }

        public async Task AddAsync(ScoreTable scoreTable)
        {
            _context.ScoreTables.Add(scoreTable);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ScoreTable scoreTable)
        {

            _context.ScoreTables.Update(scoreTable);
            await _context.SaveChangesAsync();
        }
        public Task<List<ScoreTable>> GetAllByUserIdAsync(string userId)
        {
            return _context.ScoreTables.Where(s => s.UserId == userId).ToListAsync();
        }
        public async Task<int> CountByUserIdAsync(string userId)
        {
            return await _context.ScoreTables.CountAsync(s => s.UserId == userId);
        }
        public async Task<bool> CheckCategoryInScoreTable(string userId, int categoryId)
        {
            return await _context.ScoreTables
                .AnyAsync(s => s.CategoryId == categoryId && s.UserId == userId);
        }
        public async Task<double> GetAverageScoreByCategoryAsync(string userId, int exerciseId, int categoryId)
        {
            var averageScore = await _context.ScoreTables
                .Where(s => s.UserId == userId && s.ExerciseId == exerciseId && s.CategoryId == categoryId)
                .AverageAsync(s => s.Score); 
            return averageScore;
        }
        public async Task<double> GetAverageScoreByCategoryAsync(string userId, int categoryId)
        {
            var averageScore = await _context.ScoreTables
                .Where(s => s.UserId == userId && s.CategoryId == categoryId)
                .Select(s => s.Score) // Lấy danh sách điểm
                .DefaultIfEmpty() // Đảm bảo có giá trị mặc định nếu không có điểm
                .AverageAsync(); // Tính trung bình

            return averageScore; // Có thể trả về giá trị trung bình hoặc 0 nếu không có điểm nào
        }

    }
}
