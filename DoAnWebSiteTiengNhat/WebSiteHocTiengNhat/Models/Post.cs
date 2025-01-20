using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class Post
    {
        [Key]
        public string PostId { get; set; }
        public string Title { get; set; }
        public string? thumbnail { get; set; }
        public string Content { get; set; }

        [Required]
        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }
        [JsonIgnore]
        public IdentityUser? User { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
    }
}
