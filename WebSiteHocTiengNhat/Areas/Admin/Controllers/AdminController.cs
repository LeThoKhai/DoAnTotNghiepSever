using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
       
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> CreateAdminAccount()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = new IdentityUser
            {
                UserName = "admin@gmail.com",//Thay đổi tùy ý
                Email = "admin@gmail.com"//Thay đối tùy ý
            };
            var result = await _userManager.CreateAsync(user, "Hutech@123");//Chú ý format mật khẩu if (result.Succeeded)
            if (result.Succeeded)
            {
                var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (addToRoleResult.Succeeded)
                {
                    return Content("Admin account created and assigned to Admin role successfully!");
                }
                await _userManager.CreateAsync(user, "Admin");
                return Content("Admin account created successfully!");
            }
            return BadRequest("Failed to create admin account");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
