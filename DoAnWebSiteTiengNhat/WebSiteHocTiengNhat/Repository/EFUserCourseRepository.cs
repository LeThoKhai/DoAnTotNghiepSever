using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repositories
{
    public class EFUserCourseRepository : IUserCourseRepository
    {
        private readonly ApplicationDbContext _context;

        public EFUserCourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserToCourseAsync(UserCourse userCourse)
        {
            await _context.UserCourses.AddAsync(userCourse);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserCourse>> GetUserCoursesByUserIdAsync(string userId)
        {
            return await _context.UserCourses
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Course)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserCourse>> GetUserCoursesByCourseIdAsync(int courseId)
        {
            return await _context.UserCourses
                .Where(uc => uc.CourseId == courseId)
                .Include(uc => uc.User)
                .ToListAsync();
        }
        public async Task<IEnumerable<UserCourse>> GetUserCoursesByCourseIdAndUserIdAsync(int courseId, string userId)
        {
            return await _context.UserCourses
                .Where(uc => uc.CourseId == courseId&&uc.UserId==userId)
                .Include(uc => uc.User)
            .ToListAsync();
        }
        public async Task<UserCourse> GetByUserIdAndCourseIdAsync(string? userId, int courseId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
            }

            if (courseId <= 0)
            {
                throw new ArgumentException("CourseId must be a positive integer.", nameof(courseId));
            }

            var userCourse = await _context.UserCourses
                .Where(uc => uc.UserId == userId && uc.CourseId == courseId)
                .Include(uc => uc.Course)
                .FirstOrDefaultAsync();

            if (userCourse == null)
            {               
                return null; // Hoặc có thể trả về một đối tượng mặc định
            }

            return userCourse;
        }



        public async Task<IEnumerable<UserCourse>> GetAllUserCoursesAsync()
        {
            return await _context.UserCourses
                .Include(uc => uc.User)
                .Include(uc => uc.Course)
                .ToListAsync();
        }

        public async Task DeleteUserCourseAsync(UserCourse userCourse)
        {
            _context.UserCourses.Remove(userCourse);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(UserCourse userCourse)
        {
            _context.UserCourses.Update(userCourse);  
            await _context.SaveChangesAsync();       
        }
    }
}
