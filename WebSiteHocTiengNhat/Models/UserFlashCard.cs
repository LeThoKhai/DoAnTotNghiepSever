using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class UserFlashCard
    {
        [Key]
        public int? CardId { get; set; }
        public string CardFront { get; set; }
        public string CardBack { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
