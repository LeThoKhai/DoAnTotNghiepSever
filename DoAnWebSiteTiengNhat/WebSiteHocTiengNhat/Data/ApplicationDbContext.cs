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

        public DbSet<QuestionType> QuestionTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<QuestionType>().HasData(
            new QuestionType { QuestionTypeId = "QT1", QuestionTypeName = "FillInBlanks" },
            new QuestionType { QuestionTypeId = "QT2", QuestionTypeName = "McSingleAnswer" },
            new QuestionType { QuestionTypeId = "QT3", QuestionTypeName = "ReOrderParagraphs" },
            new QuestionType { QuestionTypeId = "QT4", QuestionTypeName = "McMultipleAnswer" },
            new QuestionType { QuestionTypeId = "QT5", QuestionTypeName = "WriteFromDictation" },
            new QuestionType { QuestionTypeId = "QT6", QuestionTypeName = "AnalysisText" },
            new QuestionType { QuestionTypeId = "QT7", QuestionTypeName = "WriteEssay" },
            new QuestionType { QuestionTypeId = "QT8", QuestionTypeName = "RepeatSentence" },
            new QuestionType { QuestionTypeId = "QT9", QuestionTypeName = "DescribeImage" },
            new QuestionType { QuestionTypeId = "QT10", QuestionTypeName = "AnswerShortQuestion" }
            );
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
