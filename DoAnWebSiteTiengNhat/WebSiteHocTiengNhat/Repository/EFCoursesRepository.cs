using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public class EFCoursesRepository : ICoursesRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCoursesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
           
        }
        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses.FindAsync(id);
        }
        public async Task AddAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            _context.Dispose();
        }
        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

        }
        public async Task DeleteAsync(int id)
        {
            var Courses = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(Courses);
            await _context.SaveChangesAsync();

        }

    }
}
