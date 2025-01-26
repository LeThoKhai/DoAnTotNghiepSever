using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Data;

namespace WebSiteHocTiengNhat.Repository
{
    public class EFCategoryQuestionRepository : ICategoryQuestionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IQuestionRepository _questionRepository;
        public EFCategoryQuestionRepository(ApplicationDbContext context, IQuestionRepository questionRepository)
        {
            _context = context;
            _questionRepository = questionRepository;
        }
        public async Task<IEnumerable<CategoryQuestion>> GetAllAsync()
        {
            return await _context.CategoryQuestions.ToListAsync();
        }

        public async Task<CategoryQuestion> GetByIdAsync(int? id)
        {
            return await _context.CategoryQuestions.FindAsync(id);
        }
        //public async Task<IEnumerable<Question>> GetListQuestions(int? categoryQuestionId)
        //{
        //    var list = await _questionRepository.GetAllAsync();

        //    return list.Where(n => n.CategoryQuestionId == categoryQuestionId);
        //}
        public async Task<List<Question>> GetByCategoryQuestionId(int? categoryquestionId)
        {
            if (categoryquestionId == null) return new List<Question>();
            var questions = await _context.Questions.Where(n => n.CategoryQuestionId == categoryquestionId).ToListAsync();
            return questions;
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
       /* public async Task<int?> GetCategoryQuestionIdReading()
        {
            var categoryQuestions = await GetAllAsync();
            var readingCategoryQuestion = categoryQuestions.FirstOrDefault(n => n.IsReading);
            return readingCategoryQuestion?.CategoryQuestionId;
        }*/

    }
}
