using System.ComponentModel.DataAnnotations;

namespace WebSiteHocTiengNhat.Models
{
    public class QuestionType
    {
        [Key]
        public string QuestionTypeId { get; set; }
        [Required]
        public string QuestionTypeName { get; set; }
    }
}
