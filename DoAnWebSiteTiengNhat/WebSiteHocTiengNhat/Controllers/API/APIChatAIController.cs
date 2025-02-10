using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly string _ollamaApiUrl = "http://localhost:11434/api/chat";
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

        // Tạo payload theo cấu trúc yêu cầu của API
        var requestBody = new
        {
            model = "llama3.2:1b", // Tên mô hình
            messages = new[]
            {
                new { role = "user", content = "Đóng vai bạn là một giảng viên dạy tiếng nhật thân thiện tên là Manabihub, chỉ trả lời các câu hỏi ngắn gọn liên quan đến tiếng nhật," +
                "nếu đã giới thiệu rồi thì không cần giới thiệu lại" + userQuestion }
            }
        };

        var jsonContent = JsonConvert.SerializeObject(requestBody);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _ollamaApiUrl)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        try
        {
            var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"API Error: {errorContent}");
            }

            var responseStream = await response.Content.ReadAsStreamAsync();

            using (var streamReader = new StreamReader(responseStream))
            {
                string line;
                var fullContent = new StringBuilder();

                // Đọc từng dòng JSON từ stream
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    try
                    {
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(line);
                        var content = jsonObject?["message"]?["content"]?.ToString();

                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            fullContent.Append(content);
                        }
                    }
                    catch (JsonReaderException)
                    {
                        // Nếu dòng không phải JSON hợp lệ, bỏ qua
                        continue;
                    }
                }

                return Ok(fullContent.ToString());
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi xảy ra khi gọi API: {ex.Message}");
        }
    }
}
