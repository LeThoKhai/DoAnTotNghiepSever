using System.ComponentModel.DataAnnotations;

namespace WebSiteHocTiengNhat.Models
{
    public class SpeakingQuestion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string QuestionContent { get; set; }
        [Required]
        public string Answer { get; set; }
        [Required]
        public int Level { get; set; }
        public string? Link { get; set; }
    }
}
