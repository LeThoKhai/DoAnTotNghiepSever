using Microsoft.AspNetCore.Mvc;

namespace WebSiteHocTiengNhat.Controllers
{
    public class UserFlashCardsController : Controller
    {
        public IActionResult Index(int courseId)
        {

            return View();
        }
    }
}
