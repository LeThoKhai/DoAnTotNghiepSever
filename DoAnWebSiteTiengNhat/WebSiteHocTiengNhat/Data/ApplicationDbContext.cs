using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Question> Questions{ get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<FlashCard> FlashCards { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ScoreTable> ScoreTables { get; set; }
        public DbSet<Post> Posts {  get; set; }
        public DbSet<CategoryQuestion> CategoryQuestions { get; set; }
        public DbSet<Certificate> Certificates { get; set; }



        //tạo bảng gồm 2 khóa chính
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure composite primary key
            builder.Entity<UserCourse>()
                .HasKey(uc => new { uc.CourseId, uc.UserId });

            // Configure relationships and other settings if needed

            builder.Entity<ScoreTable>()
                .HasOne(s => s.Exercise)
                .WithMany()
                .HasForeignKey(s => s.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa theo chuỗi

            // Cấu hình khóa ngoại cho ScoreTable -> Category
            builder.Entity<ScoreTable>()
                .HasOne(s => s.Category)
                .WithMany()
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa theo chuỗi

            // Cấu hình khóa ngoại cho ScoreTable -> User (AspNetUsers)
            builder.Entity<ScoreTable>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa theo chuỗi
        }
    }
}
