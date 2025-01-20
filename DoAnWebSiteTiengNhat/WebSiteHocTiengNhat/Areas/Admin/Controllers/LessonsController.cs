using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.ViewModels;
using System.Text.RegularExpressions;
namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LessonsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly string _geminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
        private readonly string _apiKey = "AIzaSyDkh-E-q9D9RVA0mzxtPc35WTY9JjxyeoI";

        private readonly ICoursesRepository _coursesRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly ILessonRepository _lessonRepository;

        public LessonsController(
            HttpClient httpClient,
            ApplicationDbContext context,
            ICoursesRepository coursesRepository,
            ICategoryRepository categoryRepository,
            IFlashCardRepository flashCardRepository,
            ILessonRepository lessonRepository)
        {
            _httpClient = httpClient;
            _context = context;
            _coursesRepository = coursesRepository;
            _categoryRepository = categoryRepository;
            _lessonRepository = lessonRepository;
            _flashCardRepository= flashCardRepository;
        }

        // Tạo flashcards từ bài học
        [HttpPost("generate-flashcards")]
        public async Task<IActionResult> GenerateFlashCards(Lesson lessons)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessons.LessonId);
            if (lesson == null)
            {
                return NotFound($"Lesson với ID {lessons.LessonId} không tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(lesson.Content))
            {
                return BadRequest("Lesson không có nội dung.");
            }

            // Gọi hàm GenerateFlashCardContent để tạo danh sách flashcard
            var flashCardData = await GenerateFlashCardContent(lessons);
            if (flashCardData == null || !flashCardData.Any())
            {
                return StatusCode(500, "Không thể tạo flashcard từ AI hoặc danh sách rỗng.");
            }

            foreach (var flashCard in flashCardData)
            {
                flashCard.LessonId = lessons.LessonId; 
                await _flashCardRepository.AddAsync(flashCard);
            }

            await _context.SaveChangesAsync();
            return Ok(flashCardData);
        }

        private async Task<List<FlashCard>?> GenerateFlashCardContent(Lesson lesson)
        {
            string content = FormatHtmlToSingleRow(lesson.Content);
            try
            {
                // Tạo request body
                var requestBody = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = $"Hãy tạo ít nhất 5-8 flashcard dựa trên việc tóm tắt ngắn gọn ý chính dựa trên nội dung sau: {content}.Trả về định dạng JSON với các thuộc tính: CardName, CardFront, CardBack, LessonId={lesson.LessonId}," +
                            $"loại bỏ các ký tự đặc biệt, chỉ trả về chuỗi JSON thuần túy không chú thích hay đóng ngoặc,loại bỏ chữ '''json khi trả về vì tôi không cần. phải có thuộc tính LessonId"
                        }
                    }
                }
            }
                };

                // Serialize request body
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var apiUrlWithKey = $"{_geminiApiUrl}?key={_apiKey}";

                // Gửi request đến API
                var response = await _httpClient.PostAsync(apiUrlWithKey, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    // Đọc dữ liệu JSON trả về
                    var responseData = await response.Content.ReadAsStringAsync();

                    // Deserialize dữ liệu trả về thành đối tượng JSON động
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseData);

                    // Lấy phần text chứa chuỗi JSON cần thiết
                    var text = jsonObject?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        Console.WriteLine("Không có nội dung hợp lệ trong phản hồi.");
                        return null;
                    }

                    Console.WriteLine($"Dữ liệu JSON nhận được: {text}");

                    // Kiểm tra nếu chuỗi JSON trả về là mảng
                    try
                    {
                        var flashCards = JsonConvert.DeserializeObject<List<FlashCard>>(text);
                        return flashCards;
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"Lỗi khi deserialize chuỗi JSON: {jsonEx.Message}");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine($"API trả về lỗi: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xảy ra trong quá trình xử lý: {ex.Message}");
                return null;
            }
        }


        public string FormatHtmlToSingleRow(string? inputHtml)
        {
            string formattedHtml = Regex.Replace(inputHtml, @"<p[^>]*>", "").Replace("</p>", " ");
            formattedHtml = Regex.Replace(formattedHtml, @"<ul[^>]*>", "").Replace("</ul>", " ");
            formattedHtml = Regex.Replace(formattedHtml, @"<ol[^>]*>", "").Replace("</ol>", " ");
            formattedHtml = Regex.Replace(formattedHtml, @"<li[^>]*>", "").Replace("</li>", " ");
            formattedHtml = Regex.Replace(formattedHtml, @"<strong[^>]*>", "").Replace("</strong>", "");
            return formattedHtml;
        }

        // Các chức năng quản lý bài học
        public async Task<IActionResult> Index()
        {
            var courses = await _coursesRepository.GetAllAsync();
            return View(courses);
        }

        public async Task<IActionResult> LessonList(int courseId, int? categoryId)
        {
            var lessons = await _lessonRepository.GetAllAsync();
            var filteredLessons = lessons.Where(l => l.CourseId == courseId);

            if (categoryId.HasValue)
            {
                filteredLessons = filteredLessons.Where(l => l.CategoryId == categoryId.Value);
            }

            var course = await _coursesRepository.GetByIdAsync(courseId);
            var categories = await _categoryRepository.GetAllAsync();

            var viewModel = new LessonsViewModel
            {
                CourseName = course?.CourseName,
                CourseId = courseId,
                Lessons = filteredLessons.ToList(),
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create(int courseId)
        {
            var lesson = new Lesson { CourseId = courseId };
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View(lesson);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                await _lessonRepository.AddAsync(lesson);
                await GenerateFlashCards(lesson); // Đợi tạo FlashCards hoàn tất
                return RedirectToAction(nameof(LessonList), new { courseId = lesson.CourseId });
            }

            return RedirectToAction(nameof(LessonList), new { courseId = lesson.CourseId });

        }

        public async Task<IActionResult> Update(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            var categories = await _categoryRepository.GetAllAsync();

            if (lesson == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View(lesson);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Lesson lesson)
        {
            if (id != lesson.LessonId)
            {
                return BadRequest();
            }

            await _lessonRepository.UpdateAsync(lesson);
            await GenerateFlashCards(lesson); // Đợi tạo FlashCards hoàn tất
            return RedirectToAction(nameof(LessonList), new { courseId = lesson.CourseId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            await _lessonRepository.DeleteAsync(id);
            return RedirectToAction(nameof(LessonList), new { courseId = lesson.CourseId });
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int Id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(Id);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }
    }
}