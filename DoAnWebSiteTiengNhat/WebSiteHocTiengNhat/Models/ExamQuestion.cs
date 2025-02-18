using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebSiteHocTiengNhat.Models
{
    public class ExamQuestion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string QuestionContent { get; set; }
        [Required]
        public string? Link { get; set; }
        [Required]
        public string? OptionA { get; set; }
        [Required]
        public string? OptionB { get; set; }
        [Required]
        public string? OptionC { get; set; }
        [Required]
        public string? OptionD { get; set; }
        [Required]
        public string CorrectAnswer { get; set; }

        [Required]
        [ForeignKey("CategoryQuestion")]
        public int CategoryQuestionId { get; set; }
        [JsonIgnore]
        public CategoryQuestion? CategoryQuestion { get; set; }


        [Required]
        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        [JsonIgnore]
        public Exam? Exam { get; set; }
    }
}
