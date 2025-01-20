using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Detail(string id)
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
            var users = from u in _userManager.Users
                        select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.UserName.Contains(searchString) || s.Email.Contains(searchString));
            }

            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                var adminUserIds = await _userManager.GetUsersInRoleAsync(adminRole.Name);
                users = users.Where(u => !adminUserIds.Contains(u));
            }

            return View(users);
        }


    }
}
