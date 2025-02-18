using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Models3;

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
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<FlashCard> FlashCards { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<CategoryQuestion> CategoryQuestions { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions{ get; set; }
        public DbSet<ListeningQuestion> ListeningQuestions{ get; set; }
        public DbSet<ReadingQuestion> ReadingQuestions{ get; set; }
        public DbSet<SpeakingQuestion> SpeakingQuestions { get; set; }
        public DbSet<WritingQuestion> WritingQuestions{ get; set; }


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
            builder.Entity<CategoryQuestion>().HasData(
            new CategoryQuestion {CategoryQuestionId=1, CategoryQuestionName= "Đọc hiểu", IsReading = true },
            new CategoryQuestion {CategoryQuestionId=2, CategoryQuestionName = "Nghe hiểu", IsListening = true },
            new CategoryQuestion {CategoryQuestionId=3, CategoryQuestionName = "Ngữ pháp từ vựng", IsGrammarVocabulary = true }
            );
            builder.Entity<Category>().HasData(
            new Category {CategoryId=1, CategoryName= "Ngữ pháp" },
            new Category {CategoryId=2, CategoryName = "Từ vựng" },
            new Category {CategoryId=3, CategoryName = "Hán tự" },
            new Category {CategoryId=4, CategoryName = "Kỳ thi" }
            );
            // Configure composite primary key
            builder.Entity<UserCourse>()
                .HasKey(uc => new { uc.CourseId, uc.UserId });

            // Configure relationships and other settings if needed

         
        }
    }
}
