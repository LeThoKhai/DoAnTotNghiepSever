using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repositories;
using WebSiteHocTiengNhat.Repository;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IScoreTableRepository _scoreTableRepository;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IUserCourseRepository _userCourseRepository;
        private readonly IAI_Repository _aiRepository;
        private readonly IMemoryCache _memoryCache;
        public APIExercisesController(IExercisesRepository repository, IQuestionRepository questionRepository, IScoreTableRepository scoreTableRepository,
            IExercisesRepository exercisesRepository, IUserCourseRepository userCourseRepository, IAI_Repository aiRepository,IMemoryCache memoryCache)
        {
            _repository = repository;
            _questionRepository = questionRepository;
            _scoreTableRepository = scoreTableRepository;
            _exercisesRepository = exercisesRepository;
            _userCourseRepository = userCourseRepository;
            _aiRepository = aiRepository;
            _memoryCache = memoryCache;
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

        [HttpGet("course/{courseId}/category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExerciseByCourseIdAndCategoryId(int courseId, int categoryId)
        {
            var exercises = await _repository.GetByCourseIdAndCategoryIdAsync(courseId, categoryId);

            if (exercises == null || exercises.Count == 0)
            {
                return NotFound();
            }
             
            return Ok(exercises);
        }


        [HttpPost]
        public async Task<ActionResult<Exercise>> PostExercise([FromBody] Exercise exercise)
        {
            // Kiểm tra tính hợp lệ của ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Thêm bài tập mới vào cơ sở dữ liệu
            await _exercisesRepository.AddAsync(exercise);

            // Trả về trạng thái Created cùng với đường dẫn đến bài tập vừa được tạo
            return CreatedAtAction(nameof(GetExercise), new { id = exercise.ExerciseId }, exercise);
        }

        // PUT: api/APIExercises/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExercise(int id, [FromBody] Exercise exercise)
        {
            if (id != exercise.ExerciseId)
            {
                return BadRequest("Exercise ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.UpdateAsync(exercise);
            return NoContent();
        }

        // DELETE: api/APIExercises/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var exercise = await _repository.GetByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
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

        [HttpPost("/submitquestion")]
        public async Task<ActionResult<Reponsive>> SubmitAnswerForOneQuestion([FromBody] UserAnswer userAnswer)
        {
            if (userAnswer != null)
            {
                var reponsive = await _questionRepository.CaculateScore(userAnswer);
                return reponsive;
            }
            else
            {
                return NotFound(); 
            }
        }










        //Ky thi cho app se phat trien sau
        // API để nhận danh sách câu trả lời và tính điểm 
        [HttpPost("{exerciseId}/submitlistquestion")]
        public async Task<ActionResult<int>> SubmitAnswers(int exerciseId, [FromBody] List<AnswerSubmission> submissions)
        {
            var userId = User.FindFirst("userId")?.Value;
            var exercise = await _repository.GetByIdAsync(exerciseId);
            if (exercise == null) return NotFound("Exercise not found.");

            var questions = await _questionRepository.GetByExerciseId(exerciseId);
            if (questions == null || !questions.Any()) return NotFound("No questions found for this exercise.");

            var questionsString = SerializeQuestions(questions);
            var submissionsString = JsonSerializer.Serialize(submissions);

            int score = CalculateScore(questions, submissions);
            await SaveScoreAsync(exerciseId, userId, exercise.CategoryId, score);
            await UpdateUserProgressAsync(userId, exercise.CourseId);
            CacheQuestionsAndSubmissions(questionsString, submissionsString);

            return Ok(score);
        }
        [HttpGet("/explain")]
        public async Task<ActionResult<string>> Explain()
        {
            string questionsString = null;
            string submissionsString = null;
            if (_memoryCache.TryGetValue("questions", out questionsString) &&
                _memoryCache.TryGetValue("submissions", out submissionsString))
            {
                string result = await _aiRepository.SendMessage(questionsString, submissionsString);
                return Ok(result);
            }           
            return NotFound();
        }

        private string SerializeQuestions(IEnumerable<Question> questions) =>
            string.Join("\n", questions.Select(q =>
                $"QuestionId: {q.QuestionId}, Text: {q.QuestionText}, " +
                $"A: {q.OptionA}, B: {q.OptionB}, C: {q.OptionC}, D: {q.OptionD}, " +
                $"CorrectAnswer: {q.CorrectAnswer}"));

        private int CalculateScore(IEnumerable<Question> questions, IEnumerable<AnswerSubmission> submissions) =>
            questions.Count(q => submissions.Any(a => a.QuestionId == q.QuestionId && a.SelectedAnswer == q.CorrectAnswer));

        private async Task SaveScoreAsync(int exerciseId, string userId, int categoryId, int score)
        {
            var existingScore = await _scoreTableRepository.GetByExerciseIdAndUserIdAsync(exerciseId, userId);
            if (existingScore != null)
            {
                existingScore.Score = score;
                await _scoreTableRepository.UpdateAsync(existingScore);
            }
            else
            {
                var newScore = new ScoreTable
                {
                    ExerciseId = exerciseId,
                    UserId = userId,
                    CategoryId = categoryId,
                    Score = score
                };
                await _scoreTableRepository.AddAsync(newScore);
            }
        }

        private async Task UpdateUserProgressAsync(string userId, int courseId)
        {
            if (userId == null) return;

            var userCourse = await _userCourseRepository.GetByUserIdAndCourseIdAsync(userId, courseId);
            if (userCourse == null) return;

            int totalExercises = await _exercisesRepository.CountByCourseIdAsync(courseId);
            int completedExercises = await _scoreTableRepository.CountByUserIdAsync(userId);

            userCourse.progress = totalExercises > 0 ? (double)completedExercises / totalExercises * 100 : 0;
            await _userCourseRepository.UpdateAsync(userCourse);
        }
        private void CacheQuestionsAndSubmissions(string questionsString, string submissionsString)
        {
            _memoryCache.Set("questions", questionsString, TimeSpan.FromHours(1)); // Cache tồn tại trong 1 giờ
            _memoryCache.Set("submissions", submissionsString, TimeSpan.FromHours(1)); // Cache tồn tại trong 1 giờ
        }

    }

    public class AnswerSubmission
    {
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; }
    }

}
