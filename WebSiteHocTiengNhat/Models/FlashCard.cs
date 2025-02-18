using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class FlashCard
    {
        [Key]
        public int? CardId { get; set; }
        [Required]
        public string? CardName { get; set; }
        [Required]
        public string CardFront{ get; set; }
        [Required]
        public string CardBack { get; set; }
        [Required]
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        [JsonIgnore]
        public Lesson? Lesson{ get; set; }
    }
}
