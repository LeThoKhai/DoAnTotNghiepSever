using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly string _geminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
    private readonly string _apiKey = "AIzaSyDkh-E-q9D9RVA0mzxtPc35WTY9JjxyeoI";

    private readonly HttpClient _httpClient;

    public ChatController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("sendMessage")]
    public async Task<IActionResult> SendMessage([FromBody] string userQuestion)
    {
        if (string.IsNullOrWhiteSpace(userQuestion))
        {
            return BadRequest("Question cannot be empty.");
        }

        // Tạo payload JSON đúng định dạng API yêu cầu
        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new { text = "Đóng vai bạn là một giảng viên dạy tiếng nhật tên là Manabihub dễ thương, hài hước và chỉ trả lời những câu hỏi liên quan đến tiếng nhật." +
                    "Câu trả lời phải có tâm huyết. Trả lời ngắn gọn không lòng vòng.Nếu đã chào hỏi giới thiệu rồi thì ko cần trả lời lại. bỏ các biểu cảm trong ngoặc đơn" },
                    new { text = userQuestion }
                }
            }
        }
        };

        var jsonContent = JsonConvert.SerializeObject(requestBody);

        // API Key được truyền qua query string
        var apiUrlWithKey = $"{_geminiApiUrl}?key={_apiKey}";

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrlWithKey)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        // Gửi yêu cầu tới API Gemini
        var response = await _httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();

            // Parse JSON trực tiếp bằng JObject
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseData);

            // Trích xuất "text" từ phản hồi JSON
            var text = jsonObject?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return Ok("Không nhận được nội dung từ AI.");
            }

            return Ok(text);
        }


        // Xử lý lỗi trả về từ API
        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
    }

}
