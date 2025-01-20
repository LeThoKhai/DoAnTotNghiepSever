// Models/Lesson.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebSiteHocTiengNhat.Models;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }
        [Required]
        public string LessonName { get; set; }
        [Required]
        public string? Link { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }


        [Required]
        public string? Content { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int? CourseId { get; set; }
        [JsonIgnore]
        public Course? Course { get; set; }

    }
}
