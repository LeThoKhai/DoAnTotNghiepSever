using WebSiteHocTiengNhat.Data;
using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;


namespace WebSiteHocTiengNhat.Repository
{
    public class EFAI_Repository : IAI_Repository
    {
        private readonly string _geminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
        private readonly string _apiKey = "AIzaSyDkh-E-q9D9RVA0mzxtPc35WTY9JjxyeoI";

        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly ILessonRepository _lessonRepository;
        public EFAI_Repository(ApplicationDbContext context, HttpClient httpClient, ILessonRepository lessonRepository)
        {
            _context = context;
            _httpClient = httpClient;
            _lessonRepository = lessonRepository;
        }

        public async Task<string> SendMessage(string question,string Submission)
        {
            if (string.IsNullOrWhiteSpace(Submission)|| string.IsNullOrWhiteSpace(question))
            {
                return "Question cannot be empty.";
            }
            string Answer="Câu hỏi là"+ question + "và đáp án là" + Submission;
            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = "Đóng vai bạn là một giảng viên dạy tiếng nhật tên là Manabihub đang chẩm điểm cho bài kiểm tra." +
                    "chỉ ra cho người dùng cần chọn đáp án nào mới là đùng và tại sao lại chọn đáp án này nếu họ chọn sai" +
                                 "Trả lời ngắn gọn không lòng vòng. bỏ các biểu cảm trong ngoặc đơn. mọi thông tin để bạn phân tích nằm ở dưới," },
                    new { text = Answer }
                }
            }
        }
            };
            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var apiUrlWithKey = $"{_geminiApiUrl}?key={_apiKey}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrlWithKey)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseData);
                var text = jsonObject?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                if (string.IsNullOrWhiteSpace(text))
                {
                    return "Không nhận được phản hồi từ AI.";
                }
                return text;
            }
            return $"Lỗi API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
        }
        public async Task<string> SendMessageAboutLesson(string ? question, int ? lessonid)
        {

            
            string Answer = "";
            if (lessonid != null) {
                var lessoncontent = await _lessonRepository.GetByIdAsync(lessonid);
                if (lessoncontent == null)
                {
                    return $"Đã xảy ra lỗi không tìm thấy bài học tương ứng";
                }
                string formattedLesson = FormatHtmlToSingleRow(lessoncontent.Content);
                Answer = "Đây là dữ liệu để training. bạn sẽ dùng dữ liệu này để giảng bài cho học viên của tôi nếu họ ko hiểu. không trả lời bất cứ gì đến khi được hỏi" + formattedLesson;
            }
            if (question != null)
            {
                Answer= "Hãy trả lời cho tôi câu hỏi sau" + question;
            }          
            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = "Hãy đóng vai là một giảng viên thân thiện, Ưu tiên phản hồi nhanh nhất có thể ,trả lời ngắn gọn và trọng tâm. có thể dùng Feynman nếu có thể. " },
                    new { text = Answer }
                }
            }
            }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var apiUrlWithKey = $"{_geminiApiUrl}?key={_apiKey}";

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrlWithKey)
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject(responseData);
                    return jsonObject?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()
                           ?? "Không nhận được phản hồi từ AI.";
                }

                return $"Lỗi API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi: {ex.Message}";
            }
        }

        public string FormatHtmlToSingleRow(string? inputHtml)
        {
            if (string.IsNullOrWhiteSpace(inputHtml)) return string.Empty;
            string formattedHtml = Regex.Replace(inputHtml, @"<[^>]+>", " ").Trim();
            return Regex.Replace(formattedHtml, @"\s+", " ");
        }

    }
}
