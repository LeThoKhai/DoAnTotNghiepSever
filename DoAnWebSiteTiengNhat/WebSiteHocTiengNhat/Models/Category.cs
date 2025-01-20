// Models/Category.cs
using System.ComponentModel.DataAnnotations;

namespace WebSiteHocTiengNhat.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }
    }
}
