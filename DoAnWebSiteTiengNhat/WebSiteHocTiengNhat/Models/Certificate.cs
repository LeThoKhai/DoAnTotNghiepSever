using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }
        [Required]
        public string CertificateName { get; set; }
        [Required]
        public int TotelScore { get; set; } = 0;
        [Required]
        public int? Score1 { get; set; } = 0;
        [Required]
        public int? Score2 { get; set; } = 0;
        [Required]
        public int? Score3 { get; set; } = 0;
        [Required]
        public int? Score4 { get; set; } = 0;
        [Required]
        public DateTime CreatedDay { get; set; }
        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }
        [JsonIgnore]
        public IdentityUser? User { get; set; }
        public string? CreatedBy { get; set; }
    }
}
