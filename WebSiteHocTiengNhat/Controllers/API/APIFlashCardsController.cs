using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WebSiteHocTiengNhat.Data;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashCardsApiController : ControllerBase
    {
        private readonly ICoursesRepository _coursesRepository;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FlashCardsApiController(ILessonRepository lessonRepository, ICoursesRepository coursesRepository, ApplicationDbContext context,
            IFlashCardRepository flashCardRepository, IHttpClientFactory httpClientFactory, UserManager<IdentityUser> userManager)
        {
            _coursesRepository = coursesRepository;
            _lessonRepository = lessonRepository;
            _flashCardRepository = flashCardRepository;
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
            _userManager = userManager;
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

        [Authorize]
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Nội dung không được để trống.");

            // Lấy thông tin người dùng từ Claim
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Không xác định được người dùng.");

            string username = userId;

            string prompt = $"Tạo 5-8 flashcard từ đoạn sau:\n\"{content}\"\n " +
                            "Không cần bất kì câu trả lời từ bạn. tôi chỉ cần chuỗi json loại bỏ các ký tự đặc biệt, chỉ trả về chuỗi JSON thuần túy không chú thích hay đóng ngoặc,loại bỏ chữ '''json khi trả về vì tôi không cần.." +
                            "Trả về mảng JSON với thuộc tính: CardFront, CardBack.";

            var requestBody = new
            {
                model = "llama3.2:3b",
                prompt = prompt,
                stream = false
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var response = await _httpClient.PostAsync("http://localhost:11434/api/generate",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Lỗi khi gọi mô hình Ollama.");

            var responseData = await response.Content.ReadAsStringAsync();
            var jsonObj = JsonConvert.DeserializeObject<dynamic>(responseData);
            string? resultText = jsonObj?["response"]?.ToString();

            if (string.IsNullOrWhiteSpace(resultText))
                return BadRequest("Không có kết quả từ mô hình.");

            try
            {
                var flashcards = JsonConvert.DeserializeObject<List<UserFlashCard>>(resultText);

                // Gán username từ người dùng đăng nhập
                foreach (var card in flashcards)
                    card.UserName = username;

                return Ok(flashcards);
            }
            catch (JsonException ex)
            {
                return BadRequest($"Không đọc được dữ liệu JSON: {ex.Message}\nKết quả trả về:\n{resultText}");
            }
        }




        [HttpPost("save")]
        public async Task<IActionResult> SaveFlashCards([FromBody] List<UserFlashCard> flashCards)
        {
            if (flashCards == null || !flashCards.Any())
                return BadRequest("Danh sách flashcard trống.");

            try
            {
                await _context.UserFlashCards.AddRangeAsync(flashCards); 
                await _context.SaveChangesAsync();
                return Ok("Đã lưu flashcards thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lưu flashcards: {ex.Message}");
            }
        }


        [HttpGet("user-flashcards")]
        public async Task<IActionResult> GetUserFlashCards()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Không xác định được người dùng.");

            string username = userId;

            var userflashcards = await _context.UserFlashCards
                .Where(fc => fc.UserName == username)
                .ToListAsync();

            return Ok(userflashcards);
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
