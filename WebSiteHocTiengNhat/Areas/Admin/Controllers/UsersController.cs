using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Data;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbcontext;
        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbContext;
        }
        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user= _dbcontext.ApplicationUsers.FirstOrDefault(n=>n.Id==id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id,IdentityUser? u)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return View("Error", result.Errors);
            }
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Index(string searchString)
        {
            var users = _dbcontext.ApplicationUsers.ToList(); 

            // Lọc user theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.UserName.Contains(searchString) || s.Email.Contains(searchString)).ToList();
            }

            // Lọc bỏ user có role "Admin"
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync(adminRole.Name); 
                var adminUserIds = adminUsers.Select(u => u.Id).ToList(); 
                users = users.Where(u => !adminUserIds.Contains(u.Id)).ToList(); 
            }

            return View(users);
        }

        [HttpPost]
        public IActionResult ToggleVip(string id, bool isVip)
        {
            var user = _dbcontext.ApplicationUsers.FirstOrDefault(N=>N.Id==id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            user.IsVip = isVip;
            if (isVip == true)
            {
                user.VipActivatedDate = DateTime.Now;
            }
           _dbcontext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


    }
}
