// Models/Course.cs
using System.ComponentModel.DataAnnotations;

namespace WebSiteHocTiengNhat.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; }

        [Required]
        [Range(0, 10000000)]
        public decimal Price { get; set; }
        [Required]
        public string Content { get; set; }

        public bool Status { get; set; }

        public string? Image { get; set; }
        public int? quantity { get; set; } = 0;
    }
}
