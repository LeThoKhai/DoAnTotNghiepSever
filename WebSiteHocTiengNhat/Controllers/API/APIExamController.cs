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
using DocumentFormat.OpenXml.VariantTypes;

//using static WebSiteHocTiengNhat.Controllers.APIExamController.Reponsive;
namespace WebSiteHocTiengNhat.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class APIExamController : ControllerBase
    { 
        private readonly IMemoryCache _memoryCache;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public APIExamController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exam>>> GetListExam()
        {
            var list = _dbContext.Exams.ToList();
            return Ok(list);
        }
        [HttpGet("getExamByExamId/{id}")]
        public async Task<ActionResult<IEnumerable<Exam>>> GetExam(int id)
        {
            var list = _dbContext.Exams.FirstOrDefault(n=>n.ExamId==id);
            if (list == null) {
                return NotFound();
            }
            return Ok(list);
        }
        [HttpGet("getListQuestionByExamId/{examid}")]
        public async Task<ActionResult<Exercise>> GetListQuestionByExamId(int examid)
        {
            var list= _dbContext.ExamQuestions.ToList();
            var filterquestion= list.Where(n=>n.ExamId==examid).ToList();
            if (filterquestion == null)
            {
                return NotFound();
            }
            return Ok(filterquestion);
        }

        [HttpPost("/submitExamQuestion/{examId}")]
        public async Task<ActionResult<Certificate>> SubmitAnswerForExamQuestion(int examId, [FromBody] List<AnswerSubmission> submissions)
        {
            var exam= _dbContext.Exams.FirstOrDefault(n=>n.ExamId==examId);
            var listquestion = _dbContext.ExamQuestions.ToList();
            var questions=listquestion.Where(n=>n.ExamId== examId).ToList();
            double vcb = 0;
            double ls = 0;
            double rd = 0;
            double count = 0;
            if (questions != null)
            {
                foreach (var qt in questions) {
                    var sb=submissions.FirstOrDefault(n=>n.QuestionId==qt.Id);
                    var cate = _dbContext.CategoryQuestions.FirstOrDefault(n=>n.CategoryQuestionId==qt.CategoryQuestionId);
                    if (qt.CorrectAnswer.ToUpper() == sb.SelectedAnswer.ToUpper())
                    {
                        if (cate.IsGrammarVocabulary) {
                            vcb ++;
                        }
                        else if (cate.IsReading) {
                            rd ++;
                        }
                        else if (cate.IsListening) {
                            ls++;
                        }
                        else
                        {
                            BadRequest(new { Message = "Không tìm thấy category question tương ứng" });
                        }
                    }  
                    count++;
                }
                double totalscore = vcb + rd + ls;
                int check = (int)((totalscore / count) * 10);
                if (check >=  5)
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                    var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userId);
                    if (user == null)
                    {
                        return BadRequest(new { Message = "Không tìm thấy user" });
                    }
                    var ctf = new Certificate
                    {
                        CertificateName = exam.ExamName,
                        Score1 = (int)vcb,
                        Score2 = (int)rd,
                        Score3 = (int)ls,
                        TotelScore = (int)totalscore,
                        CreatedBy = "App Học tiếng Nhật Manabihub",
                        CreatedDay = DateTime.Now,
                        UserId = user.Id
                    };
                    try
                    {
                        _dbContext.Certificates.Add(ctf);
                        _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { Message = "Không thể tạo chứng chỉ", Error = ex.Message });
                    }
                    var notification = new
                    {
                        vcbscore = vcb,
                        rdscore = rd,
                        lsscore = ls,
                        ttscore=totalscore,
                        content = "Chúc mừng ! bạn đã đỗ " + exam.ExamName,
                    };
                    return Ok(notification);
                }
                var notification2 = new
                {
                    vcbscore = vcb,
                    rdscore = rd,
                    lsscore = ls,
                    ttscore = totalscore,
                    content = "Tiếc quá ! Bạn đã trượt " + exam.ExamName,
                };
                return Ok(notification2);
            }
            return BadRequest(new { Message = "Không thể xử lý" });
        }
        //private float caculatescore(double score)
        //{
        //    float caculate = 0;
        //    if (score == 0)
        //    {
        //        caculate = 0;
        //    }
        //    else if (score <= 5 && score > 0)
        //    {
        //        caculate = 1;
        //    }
        //    else if (score > 5 & score <= 8)
        //    {
        //        caculate = 2;
        //    }
        //    else
        //    {
        //        caculate = 3;
        //    }
        //    return caculate;
        //}

    }

    public class ExamAnswerSubmission
    {
        public int ExamQuestionId { get; set; }
        public string SelectedAnswer { get; set; }
    }

}
