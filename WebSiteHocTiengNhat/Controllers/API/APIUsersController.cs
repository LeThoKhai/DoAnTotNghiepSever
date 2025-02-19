using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Models;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbcontext;
    public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _roleManager= roleManager;
        _dbcontext= dbContext;
    }
    private async Task<string> SaveImage(IFormFile image)
    {
        var savePath = Path.Combine("wwwroot/images/userimages", image.FileName);
        using (var fileStream = new FileStream(savePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }
        return "/images/userimages" + image.FileName;
    }
    // Đăng ký
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest register)
    {
        // Tạo đối tượng IdentityUser với thông tin đăng ký
        var user = new ApplicationUser
        {
            UserName = register.Username,
            Email = register.Email,
            PhoneNumber= register.Phone,
            IsVip=false,
            VipActivatedDate = DateTime.UtcNow,
            Address=register.Address,
        };

        // Tạo người dùng mới với mật khẩu
        var result = await _userManager.CreateAsync(user, register.Password);

        if (result.Succeeded)
        {
            // Kiểm tra xem vai trò "Customer" đã tồn tại chưa
            var roleExists = await _roleManager.RoleExistsAsync("Customer");
            if (!roleExists)
            {
                // Tạo vai trò "Customer" nếu chưa tồn tại
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            // Gán vai trò "Customer" cho người dùng mới
            await _userManager.AddToRoleAsync(user, "Customer");

            // Trả về thông báo đăng ký thành công
            return Ok("User registered successfully");
        }

        // Trả về lỗi nếu đăng ký không thành công
        return BadRequest(result.Errors);
    }


    // Đăng nhập
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await _signInManager.PasswordSignInAsync(loginRequest.Username, loginRequest.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user != null)
            {
                var token = GenerateJwtToken(user);
                // Return token along with UserId
                return Ok(new { token });
            }

            return Unauthorized("User not found");
        }

        return Unauthorized("Invalid login attempt");
    }
    [HttpGet("userList")]
    public async Task<IActionResult> ListUser()
    {
        var users= _userManager.Users.ToList();
        var userList = new List<object>();
        foreach (var user in users)
        {
            var roles= await _userManager.GetRolesAsync(user);
            var userInfo = new
            {
                user.Id,
                user.UserName,
                user.Email,
                roles = roles
            };
            userList.Add(userInfo);
        }
        return Ok(userList);
    }


    [HttpGet("userCertificateList")]
    public async Task<IActionResult> ListCertificate()
    {
        // Lấy thông tin user
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Không được phân quyền.");
        }
        var user = await _userManager.FindByNameAsync(userId);
        if (user == null)
        {
            return NotFound("Không tìm thấy user");
        }
        var list = await _dbcontext.Certificates.Where(n => n.UserId == user.Id).ToListAsync();
        return Ok(list);
    }
    // Lấy thông tin người dùng từ token
    [HttpGet("get-user-info")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        // Lấy token từ header
        var authorizationHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return BadRequest("Thiếu token hoặc token không hợp lệ");
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        var handler = new JwtSecurityTokenHandler();

        try
        {
            // Đọc token
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "userId");

            if (userIdClaim == null)
            {
                return BadRequest("Không tìm thấy thông tin người dùng trong token");
            }

            var userId = userIdClaim.Value;

            // Tìm người dùng
            var user = await _userManager.FindByIdAsync(userId);
            var useraplication= _dbcontext.ApplicationUsers.FirstOrDefault(n=>n.Id == user.Id);
            if (useraplication == null)
            {
                return NotFound("Người dùng không tồn tại");
            }

            // Lấy các vai trò của người dùng
            var roles = await _userManager.GetRolesAsync(user);

            // Trả về thông tin người dùng
            var userInfo = new
            {
                useraplication.Id,
                useraplication.Email,
                useraplication.PhoneNumber,
                useraplication.UserName,
                useraplication.Score1,
                useraplication.Score2,
                useraplication.Score3,
                useraplication.Address,
                useraplication.VipActivatedDate,
                useraplication.IsVip,
                Roles = roles
            };

            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            return BadRequest($"Lỗi khi lấy thông tin: {ex.Message}");
        }
    }


    [HttpPost("UpdateProfile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            return Unauthorized(new { message = "Lỗi trong quá trình xác thực người dùng!" });
        }

        var user = await _dbcontext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userId);
        if (user == null)
        {
            return NotFound(new { message = "Không tìm thấy user." });
        }

        if (!string.IsNullOrEmpty(model.Email)) user.Email = model.Email;
        if (!string.IsNullOrEmpty(model.UserName)) user.UserName = model.UserName;
        if (!string.IsNullOrEmpty(model.PhoneNumber)) user.PhoneNumber = model.PhoneNumber;
        if (!string.IsNullOrEmpty(model.Address)) user.Address = model.Address;

        // Cập nhật ảnh đại diện nếu có
        if (model.ProfilePicture != null)
        {
            string imagePath = await SaveImage(model.ProfilePicture);
            user.UrlImage = imagePath;
        }

        _dbcontext.ApplicationUsers.Update(user);
        await _dbcontext.SaveChangesAsync();

        return Ok(new { message = "Thông tin người dùng đã được cập nhật!", user });
    }

    // Tạo JWT token
    private string GenerateJwtToken(IdentityUser user)
    {
        // Lấy các vai trò của người dùng
        var roles = _userManager.GetRolesAsync(user).Result;

        // Tạo danh sách các claims
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim("userId", user.Id) // Thêm UserId vào claims
    };

        // Thêm các vai trò vào claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Cấu hình key và credentials cho token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Tạo token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        // Trả về token dưới dạng chuỗi
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone {  get; set; }
        public string Address { get; set; }
    }
    public class UpdateProfileRequest
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }

}
