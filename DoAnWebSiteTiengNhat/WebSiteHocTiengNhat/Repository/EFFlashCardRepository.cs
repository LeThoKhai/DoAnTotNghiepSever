using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public class EFFlashCardRepository : IFlashCardRepository
    {
        private readonly ApplicationDbContext _context;
        public EFFlashCardRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<FlashCard>> GetAllAsync()
        {
            return await _context.FlashCards.ToListAsync();
        }
        public async Task<FlashCard> GetByIdAsync(int id)
        {
            return await _context.FlashCards.FindAsync(id);
        }
        public async Task AddAsync(FlashCard FlashCard)
        {
            _context.FlashCards.Add(FlashCard);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(FlashCard FlashCard)
        {
            _context.FlashCards.Update(FlashCard);
            //await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var FlashCard = await _context.FlashCards.FindAsync(id);
            _context.FlashCards.Remove(FlashCard);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<FlashCard>> GetByLessonIdAsync(int LessonId)
        {
            return await _context.FlashCards.Where(f => f.LessonId == LessonId).ToListAsync();
        }

    }
}
