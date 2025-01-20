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
    public async Task<IActionResult> Translate([FromQuery] string text, [FromQuery] string sourceLang, [FromQuery] string targetLang)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(sourceLang) || string.IsNullOrEmpty(targetLang))
        {
            return BadRequest("Please provide valid input parameters.");
        }

        var translatedText = await _translationService.TranslateAsync(text, sourceLang, targetLang);
        return Ok(new { TranslatedText = translatedText });
    }
}
