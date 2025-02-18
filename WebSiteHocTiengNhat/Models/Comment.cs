using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
namespace WebSiteHocTiengNhat.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public string UserName { get; set; }


        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [JsonIgnore]
        public Course? Course { get; set; }
        [JsonIgnore]
        public IdentityUser? User { get; set; }
    }
}
