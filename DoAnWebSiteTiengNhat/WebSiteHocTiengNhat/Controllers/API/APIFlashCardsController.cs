using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashCardsApiController : ControllerBase
    {
        private readonly ICoursesRepository _coursesRepository;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly ILessonRepository _lessonRepository;

        public FlashCardsApiController(ILessonRepository lessonRepository, ICoursesRepository coursesRepository, IFlashCardRepository flashCardRepository)
        {
            _coursesRepository = coursesRepository;
            _lessonRepository = lessonRepository;
            _flashCardRepository = flashCardRepository;
        }


        // GET: api/FlashCardsApi/lesson/{lessonId}/flashcards
        [HttpGet("getFlashCardByLessonId/{lessonId}")]
        public async Task<IActionResult> GetFlashCardsByLessonId(int lessonId)
        {
            var flashcards = await _flashCardRepository.GetAllAsync();
            flashcards = flashcards.Where(l => l.LessonId == lessonId);

            return Ok(flashcards);
        }

        // GET: api/FlashCardsApi/lesson/{lessonId}/flashcards
        [HttpGet("getFlashCardByName")]
        public async Task<IActionResult> GetFlashCardsByLessonId(string search)
        {
            var flashcards = await _flashCardRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                flashcards = flashcards.Where(n => n.CardName.Contains(search));
            }
            return Ok(flashcards);
        }

        // GET: api/FlashCardsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlashCardById(int id)
        {
            var flashCard = await _flashCardRepository.GetByIdAsync(id);
            if (flashCard == null)
            {
                return NotFound();
            }
            return Ok(flashCard);
        }

        //// POST: api/FlashCardsApi
        //[HttpPost]
        //public async Task<IActionResult> CreateFlashCard([FromBody] FlashCard flashCard)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _flashCardRepository.AddAsync(flashCard);
        //        return CreatedAtAction(nameof(GetFlashCardById), new { id = flashCard.CardId }, flashCard);
        //    }
        //    return BadRequest(ModelState);
        //}

        //// PUT: api/FlashCardsApi/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateFlashCard(int id, [FromBody] FlashCard flashCard)
        //{
        //    if (id != flashCard.CardId)
        //    {
        //        return BadRequest("FlashCard ID mismatch.");
        //    }

        //    var existingFlashCard = await _flashCardRepository.GetByIdAsync(id);
        //    if (existingFlashCard == null)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        existingFlashCard.CardName = flashCard.CardName;
        //        existingFlashCard.LessonId = flashCard.LessonId;
        //        existingFlashCard.CardBack = flashCard.CardBack;
        //        existingFlashCard.CardFront = flashCard.CardFront;


        //        await _flashCardRepository.UpdateAsync(existingFlashCard);
        //        return NoContent();
        //    }
        //    return BadRequest(ModelState);
        //}

        //// DELETE: api/FlashCardsApi/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFlashCard(int id)
        //{
        //    var flashCard = await _flashCardRepository.GetByIdAsync(id);
        //    if (flashCard == null)
        //    {
        //        return NotFound();
        //    }

        //    await _flashCardRepository.DeleteAsync(id);
        //    return NoContent();
        //}
    }
}
