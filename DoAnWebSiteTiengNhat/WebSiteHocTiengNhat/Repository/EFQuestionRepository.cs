using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WebSiteHocTiengNhat.Controllers;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;


namespace WebSiteHocTiengNhat.Repository
{
    public class EFQuestionRepository:IQuestionRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
        private readonly string _apiKey = "AIzaSyDkh-E-q9D9RVA0mzxtPc35WTY9JjxyeoI";
        private readonly ApplicationDbContext _context;
        public EFQuestionRepository(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions.ToListAsync();
        }
        public async Task<Question> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }
        public async Task<List<Question>> GetByExerciseId(int? exerciseId)
        {
            if (exerciseId == null) return new List<Question>();
            var questions = await _context.Questions.Where(n => n.ExerciseId == exerciseId).ToListAsync();
            return questions;
        }
        //public async Task<List<Question>> GetByCategoryQuestionId(int? categoryquestionId)
        //{
        //    if (categoryquestionId == null) return new List<Question>();
        //    var questions = await _context.Questions.Where(n => n.CategoryQuestionId == categoryquestionId).ToListAsync();
        //    return questions;
        //}
        public async Task AddAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int? id)
        {
            var question = await _context.Questions.FindAsync(id);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task<ActionResult<Reponsive>> CaculateScore(UserAnswer userAnswer)
        {
            var question = await GetByIdAsync(userAnswer.QuestionID);
            if (question == null)
            {
                return new Reponsive
                {
                    Sever_reponsive = "Question not found.",
                    Score = 0
                };
            }
            switch (question.QuestionTypeId)
            {
                case "QT1":
                    if (question.CorrectAnswerString == userAnswer.Answer)
                    {
                        Console.WriteLine($"Processing QT1...OK {question.CorrectAnswerString} == {userAnswer.Answer}");
                        return new Reponsive
                        {
                            Sever_reponsive = null,
                            Score = 1
                        };
                    }
                    else
                    {
                        return new Reponsive
                        {
                            Sever_reponsive = "Câu trả lời chưa chính xác",
                            Score = 0
                        };
                    }

                case "QT2":
                    if (question.CorrectAnswer == userAnswer.Answer)
                    {
                        Console.WriteLine($"Processing QT2...OK {question.CorrectAnswer} == {userAnswer.Answer}");
                        return new Reponsive
                        {
                            Sever_reponsive = null,
                            Score = 1
                        };
                    }
                    else
                    {
                        return new Reponsive
                        {
                            Sever_reponsive = "Câu trả lời chưa chính xác",
                            Score = 0
                        };
                    }

                    //break;
                case "QT3":
                    if (question.CorrectAnswerString == userAnswer.Answer)
                    {
                        Console.WriteLine($"Processing QT3...OK {question.CorrectAnswerString} == {userAnswer.Answer}");
                        return new Reponsive
                        {
                            Sever_reponsive = null,
                            Score = 1
                        };
                    }
                    else
                    {
                        return new Reponsive
                        {
                            Sever_reponsive = "Câu trả lời chưa chính xác",
                            Score = 0
                        };
                    }

                case "QT4":
                    if (question.CorrectAnswerString == userAnswer.Answer)
                    {
                        Console.WriteLine($"Processing QT4...OK {question.CorrectAnswerString} == {userAnswer.Answer}");
                        return new Reponsive
                        {
                            Sever_reponsive = null,
                            Score = 1
                        };
                    }
                    else
                    {
                        return new Reponsive
                        {
                            Sever_reponsive = "Câu trả lời chưa chính xác",
                            Score = 0
                        };
                    }

                case "QT5":
                    float score= CalculateSimilarity(question.CorrectAnswerString, userAnswer.Answer);
                    if(score > 0)
                    {
                        Console.WriteLine($"Processing QT5...OK {question.CorrectAnswer} == {userAnswer.Answer}");
                        return new Reponsive
                        {
                            Sever_reponsive = null,
                            Score = score
                        };
                    }
                    break;

                case "QT6":
                    string text = "Đóng vai là một giảng viên thân thiện đang chấm điểm cho bài tóm tắt này:" + userAnswer.Answer + "Dựa trên bài văn mẫu đã cho  "+ question.CorrectAnswerString + ", trả lời ngắn gọn. " +
                    "Trả về định dạng JSON với 2 thuộc tính là Score (chứa điểm mà bạn chấm từ 1-10) và Sever_reponsive (chứa lời khuyên của bạn)";
                    var result= await AnalysisText(text);
                    if (result != null)
                    {
                        return new Reponsive
                        {
                            Score = result.Score,
                            Sever_reponsive = result.Sever_reponsive,
                        };
                    }
                    break;

                case "QT7":
                    string text2 = "Đóng vai là một giảng viên thân thiện đang chấm điểm cho đoạn văn này:" + userAnswer.Answer + "Dựa trên câu hỏi đề đã cho  " + question.CorrectAnswerString + ", trả lời ngắn gọn. " +
                    "Trả về định dạng JSON với 2 thuộc tính là Score (chứa điểm mà bạn chấm từ 1-10) và Sever_reponsive (chứa lời khuyên của bạn)";
                    var result2 = await AnalysisText(text2);
                    if (result2 != null)
                    {
                        return new Reponsive
                        {
                            Score = result2.Score,
                            Sever_reponsive = result2.Sever_reponsive,
                        };
                    }
                    break;

                case "QT8":
                    float score2 = CalculateSimilarity(question.CorrectAnswerString, userAnswer.Answer);
                    if (score2 > 0)
                    {
                        Console.WriteLine($"Processing QT8...OK {question.CorrectAnswer} == {userAnswer.Answer}");
                        return new Reponsive
                        {
                            Sever_reponsive = null,
                            Score = score2
                        };
                    }
                    break;


                case "QT9":
                    string text3 = "Đóng vai là một giảng viên thân thiện đang chấm điểm cho đoạn mô tả này:" + userAnswer.Answer + "Dựa trên nội dung của hình ảnh mẫu đã cho nói về " + question.CorrectAnswerString + ", trả lời ngắn gọn. " +
                    "Trả về định dạng JSON với 2 thuộc tính là Score (chứa điểm mà bạn chấm từ 1-10) và Sever_reponsive (chứa lời khuyên của bạn)";
                    var result3 = await AnalysisText(text3);
                    if (result3 != null)
                    {
                        return new Reponsive
                        {
                            Score = result3.Score,
                            Sever_reponsive = result3.Sever_reponsive,
                        };
                    }
                    break;

                case "QT10":
                    string text4 = "Đóng vai là một giảng viên thân thiện đang chấm điểm cho câu trả lời này:" + userAnswer.Answer + "Dựa trên nội dung câu hỏi đã cho:" + question.CorrectAnswerString + ", trả lời ngắn gọn. " +
                    "Trả về định dạng JSON với 2 thuộc tính là Score (chứa điểm mà bạn chấm từ 1-10) và Sever_reponsive (chứa lời khuyên của bạn)";
                    var result4 = await AnalysisText(text4);
                    if (result4 != null)
                    {
                        return new Reponsive
                        {
                            Score = result4.Score,
                            Sever_reponsive = result4.Sever_reponsive,
                        };
                    }
                    break;

                default:
                    Console.WriteLine("Unknown QuestionTypeId");
                    break;
            }

            // Giá trị trả về mặc định nếu không xử lý được case nào
            return new Reponsive
            {
                Sever_reponsive = "Unhandled QuestionTypeId or invalid logic.",
                Score = 0
            };
        }

        public float CalculateSimilarity(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2))
                return 1;

            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0.1f;
            int n = str1.Length;
            int m = str2.Length;
            int[,] dp = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++) dp[i, 0] = i;
            for (int j = 0; j <= m; j++) dp[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }
            int distance = dp[n, m];
            int maxLength = Math.Max(n, m);
            float similarity = 1.0f - (float)distance / maxLength;

            return Math.Max(0.1f, similarity);
        }
        public async Task<Reponsive> AnalysisText(string texts)
        {
            try
            {
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
                            text = $"{texts}+loại bỏ các ký tự đặc biệt, chỉ trả về chuỗi JSON thuần túy không chú thích hay đóng ngoặc,loại bỏ chữ '''json khi trả về vì tôi không cần"
                        }
                    }
                }
            }
                };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var apiUrlWithKey = $"{_geminiApiUrl}?key={_apiKey}";
                var response = await _httpClient.PostAsync(apiUrlWithKey, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseData);
                    var text = jsonObject?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        Console.WriteLine("Không có nội dung hợp lệ trong phản hồi.");
                        return null;
                    }
                    Console.WriteLine($"Dữ liệu nhận được từ API: {responseData}");

                    try
                    {
                        var reponsive = JsonConvert.DeserializeObject<Reponsive>(text);
                        return reponsive;
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"Lỗi khi parse JSON: {jsonEx.Message}");
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

    }
    public class UserAnswer
    {
        public int QuestionID { get; set; }
        public string Answer { get; set; }

    }
    public class Reponsive
    {
        public float Score { get; set; }
        public string? Sever_reponsive { get; set; }
    }

}
