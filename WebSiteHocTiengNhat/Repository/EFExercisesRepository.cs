using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public class EFExercisesRepository:IExercisesRepository
    {
        private readonly ApplicationDbContext _context;
        public EFExercisesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<List<Exercise>> GetAllByCourseIdAsync(int courseId)
        {
            return _context.Exercises.Where(e => e.CourseId == courseId).ToListAsync();
        }
        public async Task<Exercise> GetByCourseIdAsync(int courseId)
        {
            // Kiểm tra nếu DbContext hoặc DbSet là null
            if (_context == null || _context.Exercises == null)
            {
                throw new InvalidOperationException("DbContext không được khởi tạo.");
            }
            // Lấy bài học đầu tiên thỏa mãn điều kiện courseId
            return await _context.Exercises.Where(n => n.CourseId == courseId).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Exercise>> GetListByCourseIdAsync(int courseId)
        {
            // Kiểm tra nếu DbContext hoặc DbSet là null
            if (_context == null || _context.Exercises == null)
            {
                throw new InvalidOperationException("DbContext không được khởi tạo.");
            }
            // Lấy tất cả bài tập thỏa mãn điều kiện courseId
            return await _context.Exercises
                .Where(n => n.CourseId == courseId)
                .ToListAsync();
        }
        public async Task<List<Exercise>> GetByCourseIdAndCategoryIdAsync(int courseId, int categoryId)
        {
            // Kiểm tra nếu DbContext hoặc DbSet là null
            if (_context == null || _context.Exercises == null)
            {
                throw new InvalidOperationException("DbContext không được khởi tạo.");
            }
            return await _context.Exercises
                .Where(lesson => lesson.CourseId == courseId && lesson.CategoryId == categoryId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Exercise>> GetAllAsync()
        {
            return await _context.Exercises.ToListAsync();
        }
        public async Task<Exercise> GetByIdAsync(int id)
        {
            return await _context.Exercises.FindAsync(id);
        }
        public async Task AddAsync(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Exercise exercise)
        {
            _context.Exercises.Update(exercise);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
        }
        public async Task<int> CountByCourseIdAsync(int CourseId)
        {
                return await _context.Exercises.CountAsync(s => s.CourseId == CourseId);
        }
    }
}
