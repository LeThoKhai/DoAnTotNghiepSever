using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [Required]
        public string QuestionText { get; set; }
        public string? Link { get; set; }
        [Required]
        public string? OptionA { get; set; }
        [Required]
        public string? OptionB { get; set; }
        [Required]
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        [Required]
        public string CorrectAnswer { get; set; }


        [Required]
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }
        [JsonIgnore]
        public Exercise? Exercise { get; set; }

    }
}
