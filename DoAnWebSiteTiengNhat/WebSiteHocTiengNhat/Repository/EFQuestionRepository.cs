using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public class EFQuestionRepository:IQuestionRepository
    {
        private readonly ApplicationDbContext _context;
        public EFQuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions.ToListAsync();
        }
        public async Task<Question> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }
        public async Task<List<Question>> GetByExerciseId(int? exerciseId)
        {
            if (exerciseId == null) return new List<Question>();
            var questions = await _context.Questions.Where(n => n.ExerciseId == exerciseId).ToListAsync();
            return questions;
        }
        public async Task AddAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int? id)
        {
            var question = await _context.Questions.FindAsync(id);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

    }
}
