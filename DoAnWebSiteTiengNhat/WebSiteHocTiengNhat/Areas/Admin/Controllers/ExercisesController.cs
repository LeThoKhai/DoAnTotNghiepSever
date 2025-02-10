using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2019.Excel.ThreadedComments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.ViewModels;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExercisesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
        private readonly string _apiKey = "AIzaSyDkh-E-q9D9RVA0mzxtPc35WTY9JjxyeoI";
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICoursesRepository _coursesRepository;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ICategoryQuestionRepository _categoryQuestionRepository;
        public ExercisesController(
            HttpClient httpClient, 
            IQuestionRepository questionRepository,
            ICategoryRepository categoryRepository, 
            ICoursesRepository coursesRepository, 
            IExercisesRepository exercisesRepository,
            ICategoryQuestionRepository categoryQuestionRepository)
        {
            _httpClient = httpClient;
            _categoryRepository = categoryRepository;
            _coursesRepository = coursesRepository;
            _questionRepository = questionRepository;
            _exercisesRepository = exercisesRepository;
            _categoryQuestionRepository = categoryQuestionRepository;
        }


        // Tạo flashcards từ bài học
        [HttpPost("generate-exercise")]
        public async Task<IActionResult> GenerateExercises(Exercise exercise)
        {
            var Exercise = await  _exercisesRepository.GetByIdAsync(exercise.ExerciseId);
            if (Exercise == null)
            {
                return NotFound($"Exercise với ID {exercise.ExerciseId} không tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(exercise.Content))
            {
                return BadRequest("Exercise không có nội dung.");
            }

            // Gọi hàm GenerateFlashCardContent để tạo danh sách question
            var QuestionData = await GenerateQuestionContent(exercise);
            if (QuestionData == null || !QuestionData.Any())
            {
                return StatusCode(500, "Không thể tạo Question từ AI hoặc danh sách rỗng.");
            }

            foreach (var question in QuestionData)
            {
                question.ExerciseId = exercise.ExerciseId;
                await _questionRepository.AddAsync(question);
            }

           // await _context.SaveChangesAsync();
            return Ok(QuestionData);
        }

        private async Task<List<Question>?> GenerateQuestionContent(Exercise exercise)
        {
            int? categoryQuestionId = null;
            var cq = await _categoryQuestionRepository.GetAllAsync();

            // Duyệt danh sách và gán giá trị cho categoryQuestionId
            foreach (var i in cq)
            {
                if (i.IsGrammarVocabulary)
                {
                    categoryQuestionId = i.CategoryQuestionId;
                    break;
                }
            }


            // Kiểm tra nếu categoryQuestionId không được gán
            if (categoryQuestionId == null)
            {
                Console.WriteLine("Không tìm thấy CategoryQuestionId phù hợp.");
                return null;
            }

            string content = FormatHtmlToSingleRow(exercise.Content);
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
                            text = $"Hãy tạo 1-10 danh sách question dựa trên việc tóm tắt ngắn gọn ý chính dựa trên nội dung sau: {content}.Trả về định dạng JSON với các thuộc tính: QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer, ExerciseId={exercise.ExerciseId}, CategoryQuestionId={categoryQuestionId}, QuestionTypeId='QT2'"+
                            $"loại bỏ các ký tự đặc biệt, chỉ trả về chuỗi JSON thuần túy không chú thích hay đóng ngoặc,loại bỏ chữ '''json khi trả về vì tôi không cần. phải có thuộc tính ExerciseId, giá trị của CorrectAnswer chỉ nhận 'A,B,C,D'"
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
                        var question = JsonConvert.DeserializeObject<List<Question>>(text);
                        return question;
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


        public async Task<IActionResult> Index()
        {
            var course = await _coursesRepository.GetAllAsync();
            return View(course);
        }
        public async Task<IActionResult> ExerciseList(int courseId, int? categoryId)
        {
            var exercise = await _exercisesRepository.GetAllAsync();
            var filteredexercise = exercise.Where(n => n.CourseId == courseId);
            if (categoryId.HasValue)
            {
                filteredexercise = filteredexercise.Where(n => n.CategoryId == categoryId);
            }
            var course = await _coursesRepository.GetByIdAsync(courseId);
            course.quantity=course.quantity ++;
            var courseName = course?.CourseName;
            var categories = await _categoryRepository.GetAllAsync();
            var categoryList = categories.Select(n => new SelectListItem
            {
                Value = n.CategoryId.ToString(),
                Text = n.CategoryName,
            }).ToList();
            var viewModel = new ExerciseViewModel
            {
                CourseId = courseId,
                CourseName = courseName,
                Exercises = filteredexercise.ToList(),
                Categories = categoryList,

            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create(int courseId)
        {
            var exercise = new Exercise { CourseId = courseId };
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            
            return View(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                await _exercisesRepository.AddAsync(exercise);
                return RedirectToAction(nameof(ExerciseList), new { courseId = exercise.CourseId });
            }

            return RedirectToAction(nameof(ExerciseList), new { courseId = exercise.CourseId });
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int Id)
        {
            var exercise = await _exercisesRepository.GetByIdAsync(Id);
            var question = await _questionRepository.GetAllAsync();
            var filterquestion= question.Where(n=>n.ExerciseId == Id);
            ViewBag.ExerciseId = exercise.ExerciseId;
            var viewmodel = new ExerciseDetailViewModel
            {
                ExerciseId = Id,
                ExerciseName = exercise.ExerciseName,
                questions = filterquestion.ToList(),
                CourseId=exercise.CourseId,
            };
            if (exercise == null)
            {
                return NotFound();
            }
            
            return View(viewmodel);
        }
        public async Task<IActionResult> Update(int id)
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var exercise = await _exercisesRepository.GetByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }
            return View(exercise);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Exercise exercise)
        {
            if (id != exercise.ExerciseId)
            {
                return NotFound();
            }
            var exs = await _exercisesRepository.GetByIdAsync(id);
            if (ModelState.IsValid)
            {
                var existingProduct = await _exercisesRepository.GetByIdAsync(id);
                existingProduct.ExerciseName = exercise.ExerciseName;
                existingProduct.CategoryId = exercise.CategoryId;
                existingProduct.Content = exercise.Content;
                await _exercisesRepository.UpdateAsync(existingProduct);
                await GenerateExercises(exercise);
                return RedirectToAction(nameof(ExerciseList), new { courseId = existingProduct.CourseId });
            }
            return RedirectToAction(nameof(ExerciseList), new { courseId = exs.CourseId });
        }
        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var exercise = await _exercisesRepository.GetByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }
            return View(exercise);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id, Exercise exercise)
        {
            var lesson = await _exercisesRepository.GetByIdAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            int idcourse = lesson.CourseId;
            await _exercisesRepository.DeleteAsync(id);
            return RedirectToAction(nameof(ExerciseList), new { courseId = idcourse });
        }
    }
}
