using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Data;

namespace WebSiteHocTiengNhat.Repository
{
    public class EFCategoryQuestionRepository : ICategoryQuestionRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCategoryQuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CategoryQuestion>> GetAllAsync()
        {
            return await _context.CategoryQuestions.ToListAsync();
        }
        public async Task<CategoryQuestion> GetByIdAsync(int? id)
        {
            return await _context.CategoryQuestions.FindAsync(id);
        }
        public async Task AddAsync(CategoryQuestion CategoryQuestion)
        {
            _context.CategoryQuestions.Add(CategoryQuestion);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(CategoryQuestion CategoryQuestion)
        {
            _context.CategoryQuestions.Update(CategoryQuestion);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var CategoryQuestion = await _context.CategoryQuestions.FindAsync(id);
            _context.CategoryQuestions.Remove(CategoryQuestion);
            await _context.SaveChangesAsync();
        }

    }
}
