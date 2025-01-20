using System.Collections.Generic;
using System.Threading.Tasks;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repositories
{
    public interface IUserCourseRepository
    {
        Task<IEnumerable<UserCourse>> GetUserCoursesByUserIdAsync(string userId);
        Task<IEnumerable<UserCourse>> GetUserCoursesByCourseIdAsync(int courseId);
        Task<IEnumerable<UserCourse>> GetAllUserCoursesAsync();
        Task<IEnumerable<UserCourse>> GetUserCoursesByCourseIdAndUserIdAsync(int courseId, string userId);
        Task AddUserToCourseAsync(UserCourse userCourse);
        Task DeleteUserCourseAsync(UserCourse userCourse);
        Task<UserCourse> GetByUserIdAndCourseIdAsync(string userId, int courseId);
        Task UpdateAsync(UserCourse userCourse);
    }
}
