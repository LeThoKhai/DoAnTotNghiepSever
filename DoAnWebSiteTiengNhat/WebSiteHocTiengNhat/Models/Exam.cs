using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebSiteHocTiengNhat.Models
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        [Required]
        public string ExamName { get; set; }

        [Required]
        public string? Content { get; set; }
        [Required]
        public DateTime ? Created { get; set; } = DateTime.Now;

    }
}
