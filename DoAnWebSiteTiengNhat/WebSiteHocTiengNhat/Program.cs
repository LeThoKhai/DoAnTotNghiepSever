using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebSiteHocTiengNhat.Data;
using WebSiteHocTiengNhat.Repositories;
using WebSiteHocTiengNhat.Repository;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


// Đăng ký DbContext với chuỗi kết nối
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// Đăng ký repository vào trong DI container
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<ICoursesRepository, EFCoursesRepository>();
builder.Services.AddScoped<IExercisesRepository, EFExercisesRepository>();
builder.Services.AddScoped<ILessonRepository, EFLessonRepository>();
builder.Services.AddScoped<IQuestionRepository, EFQuestionRepository>();
builder.Services.AddScoped<IUserCourseRepository,EFUserCourseRepository>();
builder.Services.AddScoped<IFlashCardRepository, EFFlashCardRepository>();
builder.Services.AddScoped<ICommentRepository, EFCommentRepository>();
builder.Services.AddScoped<IScoreTableRepository, EFScoreTableRepository>();
builder.Services.AddScoped<ICategoryQuestionRepository, EFCategoryQuestionRepository>();
builder.Services.AddScoped<IAI_Repository, EFAI_Repository>();
builder.Services.AddHttpClient<MyMemoryTranslationService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Cấu hình xác thực JWT
// Cấu hình JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    policy.RequireRole("Admin"));
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Cấu hình cho Swagger hỗ trợ JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer 12345abcdef\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddHttpClient<ChatController>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
//builder.WebHost.ConfigureKestrel(options =>
//{
//    //options.ListenAnyIP(5122);
//    options.ListenAnyIP(7076);
//});
var app = builder.Build();

// Sử dụng CORS trong ứng dụng
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root (e.g., http://localhost:<port>/)
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
         name: "admin",
         pattern: "{area:exists}/{controller=Home}/{action=Admin}/{id?}"
     );
});
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Hoặc
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
app.Run();