using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public class EFLessonRepository: ILessonRepository
    {
        private readonly ApplicationDbContext _context;
        public EFLessonRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Lesson>> GetAllAsync()
        {
            return await _context.Lessons.ToListAsync();
        }
        public async Task<Lesson> GetByIdAsync(int? id)
        {
            return await _context.Lessons.FindAsync(id);
        }
        public async Task<Lesson> GetByCourseId(int courseId)
        {
            if (_context == null || _context.Lessons == null)
            {
                throw new InvalidOperationException("DbContext không được khởi tạo.");
            }
            return await _context.Lessons
                .Where(lesson => lesson.CourseId == courseId)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Lesson>> GetListLessonByCourseId(int courseId)
        {
            return await _context.Lessons.Where(c => c.CourseId == courseId).ToListAsync();
        }
        public async Task<List<Lesson>> GetByCourseIdAndCategoryIdAsync(int courseId, int categoryId)
        {
            // Kiểm tra nếu DbContext hoặc DbSet là null
            if (_context == null || _context.Lessons == null)
            {
                throw new InvalidOperationException("DbContext không được khởi tạo.");
            }
            return await _context.Lessons
                .Where(lesson => lesson.CourseId == courseId && lesson.CategoryId == categoryId)
                .ToListAsync();
        }
        public async Task AddAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }
}
