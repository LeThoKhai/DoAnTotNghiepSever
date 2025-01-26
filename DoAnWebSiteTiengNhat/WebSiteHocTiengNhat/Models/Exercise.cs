using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace WebSiteHocTiengNhat.Models
{
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; }

        [Required]
        public string ExerciseName { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        [JsonIgnore]
        public Course? Course { get; set; }
        [Required]
        public bool IsExam { get; set; } = false;

    }
}
