using Microsoft.AspNetCore.Identity;

namespace WebSiteHocTiengNhat.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsVip {  get; set; } = true;
        public string Address { get; set; }
        public DateTime? VipActivatedDate { get; set; } = DateTime.Now;
        public float? Score1 { get; set; } = 0;
        public float? Score2 { get; set; } = 0;
        public float? Score3 { get; set; } = 0;
        public string? UrlImage { get; set; } 
    }
}
