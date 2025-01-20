using WebSiteHocTiengNhat.Models;
namespace WebSiteHocTiengNhat.Repository
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsByCourseIdAsync(int courseId);
        Task<Comment> AddCommentAsync(Comment comment);
        Task<bool> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin);
    }
}
