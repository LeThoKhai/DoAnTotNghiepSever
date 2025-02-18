using WebSiteHocTiengNhat.Data;
using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Models;
namespace WebSiteHocTiengNhat.Repository
{
    public class EFCommentRepository: ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByCourseIdAsync(int courseId)
        {
            return await _context.Comments.Where(c => c.CourseId == courseId).ToListAsync();
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCommentAsync(int commentId, string username, bool isAdmin)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || (comment.UserName != username && !isAdmin))
            {
                return false;
            }

            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
