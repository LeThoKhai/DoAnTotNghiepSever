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
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public APIExercisesController(IExercisesRepository repository, IQuestionRepository questionRepository, IScoreTableRepository scoreTableRepository,
            IExercisesRepository exercisesRepository, IUserCourseRepository userCourseRepository, IAI_Repository aiRepository,IMemoryCache memoryCache,ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _dbContext= dbContext;
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
        [HttpPost("/submitExam/{excerciseId}")]
        public async Task<ActionResult<Certificate>> SubmitAnswerForExamQuestion(int excerciseId, [FromBody] List<AnswerSubmission> submissions)
        {
            var ex = await _exercisesRepository.GetByIdAsync(excerciseId);

            int vcb = 0, red = 0, lis = 0;
            var question = await _questionRepository.GetByExerciseId(excerciseId);
            int countred = question.Count(n => n.CategoryQuestionId == 1);
            int countlis = question.Count(n => n.CategoryQuestionId == 2);
            int countvcb = question.Count(n => n.CategoryQuestionId == 3);
            if (question != null)
            {
                foreach(var qt in question)
                {
                    var sb = submissions.FirstOrDefault(n=>n.QuestionId==qt.QuestionId);
                    // trường hợp đáp án đúng
                    if (qt.CorrectAnswer == sb.SelectedAnswer)
                    {
                        if (qt.CategoryQuestionId == 1)
                        {
                            red++;
                        }
                        else if(qt.CategoryQuestionId == 2)
                        {
                            lis++;
                        }
                        else if(qt.CategoryQuestionId == 3)
                        {
                            vcb++;
                        }
                        else { 
                            return BadRequest(new { Message = "Không nằm trong categoryquestionid"});
                        }
                    }
                }
                int score1 = (vcb / countvcb) * 10,score2 = (red / countred) * 10,score3 = (lis / countlis) *10;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value?? User.FindFirst("sub")?.Value;
                if (userId != null) { 
                    var user= await _userManager.FindByNameAsync(userId);
                    var certificate = new Certificate
                    {
                        Score1 = score1,
                        Score2 = score2,
                        Score3 = score3,
                        TotelScore = score1 + score2 + score3,
                        CreatedBy = "App Học Tiếng Nhật ManabiHub",
                        CreatedDay = DateTime.Now,
                        UserId = user.Id,
                        User= user,
                        CertificateName=ex.ExerciseName
                    };
                    await _dbContext.Certificates.AddAsync(certificate);
                    _dbContext.SaveChanges();
                    return Ok(certificate);
                }
                else
                {
                    return BadRequest(new { Message = "Không tìm thấy user" });
                }
            }
            else
            {
                return BadRequest(new { Message = "Không tìm thấy question"});
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
