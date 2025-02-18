using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace WebSiteHocTiengNhat.Repository
{
    public class AI_Service
    {
        private readonly HttpClient _httpClient;
        private const string _ollamaApiUrl = "http://localhost:11434/api/generate";

        public AI_Service(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Reponsive2> UserAnalysisText(string question, string answer, string useranswer)
        {
            // Định dạng câu lệnh gửi đến AI
            string prompt = $"Bạn hãy đóng vai là một giảng viên dễ thương đang đánh giá ngữ pháp, từ vừng văn phong của mình dựa trên câu hỏi: \"{question}\" và đáp án đúng: \"{answer}\". " +
                            $"So sánh câu hỏi kèm đáp án đúng với câu trả lời của mình: \"{useranswer}\". Nếu gần đúng thì cho 10 điểm" + 
                            "Chỉ trả về kết quả dưới dạng JSON thuần túy: { \"Score\": số điểm 1-10, \"Server_reponsive\": \"lời khuyên ngắn gọn\" }.Mình chỉ muốn nhận về dữ liệu JSON";

            return await AIAnalysisText(prompt);
        }

        public async Task<Reponsive2> AIAnalysisText(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    model = "llama3.2:3b", // Chọn mô hình AI
                    prompt = prompt, // Gửi yêu cầu đến AI
                    stream = false // Không cần streaming
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var response = await _httpClient.PostAsync(_ollamaApiUrl, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseData);
                    var text = jsonObject?["response"]?.ToString(); // Lấy dữ liệu từ response

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        Console.WriteLine("Không có nội dung hợp lệ trong phản hồi.");
                        return null;
                    }

                    Console.WriteLine($"Dữ liệu nhận từ API: {text}");

                    try
                    {
                        var reponsive = JsonConvert.DeserializeObject<Reponsive2>(text);
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

    public class Reponsive2
    {
        public float Score { get; set; }
        public string? Server_reponsive { get; set; }
    }
}