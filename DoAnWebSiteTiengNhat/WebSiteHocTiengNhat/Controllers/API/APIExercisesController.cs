using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repositories;
using WebSiteHocTiengNhat.Repository;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using WebSiteHocTiengNhat.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.EntityFrameworkCore;

//using static WebSiteHocTiengNhat.Controllers.APIExercisesController.Reponsive;
namespace WebSiteHocTiengNhat.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    // Class để chứa thông tin câu trả lời của người dùng
    public class APIExercisesController : ControllerBase
    {

        private readonly IExercisesRepository _repository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IUserCourseRepository _userCourseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAI_Repository _aiRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public APIExercisesController(IExercisesRepository repository, IQuestionRepository questionRepository,
            IExercisesRepository exercisesRepository, IUserCourseRepository userCourseRepository, IAI_Repository aiRepository,IMemoryCache memoryCache,ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager, ICategoryRepository categoryRepository)
        {
            _userManager = userManager;
            _dbContext= dbContext;
            _repository = repository;
            _questionRepository = questionRepository;
            _exercisesRepository = exercisesRepository;
            _userCourseRepository = userCourseRepository;
            _aiRepository = aiRepository;
            _memoryCache = memoryCache;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
        {
            var exercises = await _repository.GetAllAsync();
            return Ok(exercises);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(int id)
        {
            var exercise = await _repository.GetByIdAsync(id);

            if (exercise == null)
            {
                return NotFound();
            }

            return Ok(exercise);
        }
        [HttpGet("getListExam/{courseId}")]
        public async Task<ActionResult<IEnumerable<Exercise>>> getExam(int courseId)
        {
            var exercise= await _repository.GetAllByCourseIdAsync(courseId);
            exercise = exercise.Where(n=>n.IsExam==true).ToList();
            if (exercise == null)
            {
                return NotFound();
            }
            return Ok(exercise);
        }
        [HttpGet("GetExerciseByCourse/{courseId}")]
        public async Task<ActionResult<Exercise>> GetExerciseByCourseId(int courseId)
        {
            var exercise = await _repository.GetListByCourseIdAsync(courseId);

            if (exercise == null)
            {
                return NotFound();
            }

            return Ok(exercise);
        }

        //[HttpGet("course/{courseId}/category/{categoryId}")]
        //public async Task<ActionResult<IEnumerable<Exercise>>> GetExerciseByCourseIdAndCategoryId(int courseId, int categoryId)
        //{
        //    var exercises = await _repository.GetByCourseIdAndCategoryIdAsync(courseId, categoryId);

        //    if (exercises == null || exercises.Count == 0)
        //    {
        //        return NotFound();
        //    }
             
        //    return Ok(exercises);
        //}


        //[HttpPost]
        //public async Task<ActionResult<Exercise>> PostExercise([FromBody] Exercise exercise)
        //{
        //    // Kiểm tra tính hợp lệ của ModelState
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Thêm bài tập mới vào cơ sở dữ liệu
        //    await _exercisesRepository.AddAsync(exercise);

        //    // Trả về trạng thái Created cùng với đường dẫn đến bài tập vừa được tạo
        //    return CreatedAtAction(nameof(GetExercise), new { id = exercise.ExerciseId }, exercise);
        //}

        //// PUT: api/APIExercises/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutExercise(int id, [FromBody] Exercise exercise)
        //{
        //    if (id != exercise.ExerciseId)
        //    {
        //        return BadRequest("Exercise ID mismatch.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    await _repository.UpdateAsync(exercise);
        //    return NoContent();
        //}

        //// DELETE: api/APIExercises/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteExercise(int id)
        //{
        //    var exercise = await _repository.GetByIdAsync(id);
        //    if (exercise == null)
        //    {
        //        return NotFound();
        //    }

        //    await _repository.DeleteAsync(id);
        //    return NoContent();
        //}
        [HttpGet("getQuestionListByExerciseId/{exerciseId}")]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsByExercise(int exerciseId)
        {
            var questions = await _questionRepository.GetByExerciseId(exerciseId);

            if (questions == null || !questions.Any())
            {
                return NotFound();
            }

            return Ok(questions);
        }

        [HttpPost("/submitquestion/{excerciseId}")]
        public async Task<ActionResult<Certificate>> SubmitAnswerForExamQuestion(int excerciseId, [FromBody] List<AnswerSubmission> submissions)
        {
            var ex = await _exercisesRepository.GetByIdAsync(excerciseId);
            var question = await _questionRepository.GetByExerciseId(excerciseId);
            var cate = await _categoryRepository.GetByIdAsync(ex.CategoryId);
            double score = 0;
            double count = 0;
            if (question != null)
            {
                foreach (var qt in question)
                {
                    var sb = submissions.FirstOrDefault(n => n.QuestionId == qt.QuestionId);
                    // trường hợp đáp án đúng
                    if (qt.CorrectAnswer == sb.SelectedAnswer)
                    {
                        score++;
                    }
                    count++;
                }
                score = Math.Round((score / count) * 10, 1);
                //Thêm điểm vào User
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userId); // Sử dụng FirstOrDefaultAsync
                if (user != null)
                {
                    if (cate.CategoryName == "Ngữ pháp")
                    {
                        user.Score1 += caculatescore(score);
                    }
                    else if (cate.CategoryName == "Từ vựng")
                    {
                        user.Score2 += caculatescore(score);

                    }
                    else if (cate.CategoryName == "Hán tự")
                    {
                        user.Score3 += caculatescore(score);
                    }
                    else
                    {
                        return BadRequest(new { Message = "Không tìm thấy category phù hợp" });
                    }
                    await _dbContext.SaveChangesAsync();
                    return Ok(user);
                }
                else
                {
                    return BadRequest(new { Message = "Không tìm thấy user" });
                }
            }

            return BadRequest(new { Message = "Không thể xử lý" });
        }
        private float caculatescore(double score)
        {
            float caculate = 0;
            if (score == 0)
            {
                caculate = 0;
            }
            else if (score <= 5 && score > 0) {
                caculate = 1;
            }
            else if (score > 5 & score <= 8)
            {
                caculate = 2;
            }
            else
            {
                caculate = 3;
            }
            return caculate;
        }

    }

    public class AnswerSubmission
    {
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; }
    }

}
