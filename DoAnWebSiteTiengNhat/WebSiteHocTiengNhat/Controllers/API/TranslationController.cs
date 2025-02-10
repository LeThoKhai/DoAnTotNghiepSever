using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebSiteHocTiengNhat.Repository;

[ApiController]
[Route("api/[controller]")]
public class TranslationController : ControllerBase
{
    private readonly MyMemoryTranslationService _translationService;

    public TranslationController(MyMemoryTranslationService translationService)
    {
        _translationService = translationService;
    }

    [HttpGet("translate")]
    public async Task<IActionResult> Translate([FromQuery] string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return BadRequest("Please provide valid input parameters.");
        }
        var translatedText = await _translationService.TranslateAsync(text, "ja", "vi");
        return Ok(new { TranslatedText = translatedText });
    }
}
