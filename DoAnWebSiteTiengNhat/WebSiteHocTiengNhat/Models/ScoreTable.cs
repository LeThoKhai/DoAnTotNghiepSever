using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class ScoreTable
    {
        [Key]
        public int ScoreTableId { get; set; }


        [Required]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }


        [Required]
        public string UserId { get; set; }
        [JsonIgnore]
        public IdentityUser? User { get; set; }

        [Required]
        public int ExerciseId { get; set; }
        [JsonIgnore]
        public Exercise? Exercise { get; set; }

        [Required]
        public int Score { get; set; }
    }
}
