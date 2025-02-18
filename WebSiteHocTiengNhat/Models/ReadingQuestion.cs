using System.ComponentModel.DataAnnotations;

namespace WebSiteHocTiengNhat.Models
{
    public class ReadingQuestion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string QuestionContent { get; set; }
        [Required]
        public string Answer { get; set; }
        [Required]
        public int Level { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? Link { get; set; }
    }
}
