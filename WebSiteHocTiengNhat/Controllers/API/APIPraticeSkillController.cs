using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSiteHocTiengNhat.Controllers;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class APIPraticeSkillController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AI_Service _aiService;
        public APIPraticeSkillController(ApplicationDbContext dbContext,AI_Service aI_Service)
        {
            _dbContext = dbContext;
            _aiService = aI_Service;
        }
        ////GET///
        [HttpGet("getListeningQuestion")]
        public async Task<IActionResult> ListeningQuestion()
        {
            var list = await _dbContext.ListeningQuestions.ToListAsync();
            return Ok(list);
        }
        [HttpGet("getReadingQuestion")]
        public async Task<IActionResult> ReadingQuestion()
        {
            var list = await _dbContext.ReadingQuestions.ToListAsync();
            return Ok(list);
        }
        [HttpGet("getSpeakingQuestion")]
        public async Task<IActionResult> SpeakingQuestion()
        {
            var list = await _dbContext.SpeakingQuestions.ToListAsync();
            return Ok(list);
        }
        [HttpGet("getWritingQuestion")]
        public async Task<IActionResult> WritingQuestion()
        {
            var list = await _dbContext.WritingQuestions.ToListAsync();
            return Ok(list);
        }

        ///POST///
        [HttpPost("submitSpeakingQuestion")]
        public async Task<Reponsive2> SubmitSpeakingQuestion(UserAnswer userAnswer)
        {
            var question= _dbContext.SpeakingQuestions.FirstOrDefault(n=>n.Id== userAnswer.QuestionId);
            if (question != null) {
                string questioncontent = FormatHtmlToSingleRow(question.QuestionContent);
                var rp = await _aiService.UserAnalysisText(questioncontent, question.Answer, userAnswer.Answer);
                return new Reponsive2 { Score = rp.Score, Server_reponsive = rp.Server_reponsive };
            }
            return new Reponsive2 { Score = 0, Server_reponsive = "Dữ liệu không hợp lệ" };
        }
        [HttpPost("submitWritingQuestion")]
        public async Task<Reponsive2> SubmitWritingQuestion(UserAnswer userAnswer)
        {
            var question = _dbContext.WritingQuestions.FirstOrDefault(n => n.Id == userAnswer.QuestionId);
            if (question != null)
            {
                string questioncontent = FormatHtmlToSingleRow(question.QuestionContent);
                var rp = await _aiService.UserAnalysisText(questioncontent, question.Answer, userAnswer.Answer);
                return new Reponsive2 { Score = rp.Score, Server_reponsive = rp.Server_reponsive };
            }
            return new Reponsive2 { Score = 0, Server_reponsive = "Dữ liệu không hợp lệ" };
        }
        [HttpPost("submitListeningQuestion")]
        public async Task<Reponsive2> SubmitListeningQuestion(UserAnswer userAnswer)
        {
            var question = _dbContext.ListeningQuestions.FirstOrDefault(n => n.Id == userAnswer.QuestionId);
            if (question != null)
            {
                if (userAnswer.Answer.ToUpper() == question.Answer.ToUpper())
                {
                    return new Reponsive2 { Score = 10, Server_reponsive = "Hoàn toàn chính xác"};
                }
                return new Reponsive2 { Score = 0, Server_reponsive = "Câu trả lời không chính xác" };
            }
            return new Reponsive2 { Score = 0, Server_reponsive = "Không tìm thấy câu hỏi tương ứng" };
        }
        [HttpPost("submitReadingQuestion")]
        public async Task<Reponsive2> SubmitReadingQuestion(UserAnswer userAnswer)
        {
            var question = _dbContext.ReadingQuestions.FirstOrDefault(n => n.Id == userAnswer.QuestionId);
            if (question != null)
            {
                if (userAnswer.Answer.ToUpper() == question.Answer.ToUpper())
                {
                    return new Reponsive2 { Score = 10, Server_reponsive = "Hoàn toàn chính xác" };
                }
                return new Reponsive2 { Score = 0, Server_reponsive = "Câu trả lời không chính xác" };
            }
            return new Reponsive2 { Score = 0, Server_reponsive = "Không tìm thấy câu hỏi tương ứng" };
        }



        private string FormatHtmlToSingleRow(string? inputHtml)
        {
            string formattedHtml = Regex.Replace(inputHtml, @"<p[^>]*>", "").Replace("</p>", " ");
            formattedHtml = Regex.Replace(formattedHtml, @"<ul[^>]*>", "").Replace("</ul>", " ");
            formattedHtml = Regex.Replace(formattedHtml, @"<ol[^>]*>", "").Replace("</ol>", " ");
            formattedHtml = Regex.Replace(formattedHtml, @"<li[^>]*>", "").Replace("</li>", " ");
            formattedHtml = Regex.Replace(formattedHtml, @"<strong[^>]*>", "").Replace("</strong>", "");
            return formattedHtml;
        }
    }
    public class UserAnswer
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }
    }
}