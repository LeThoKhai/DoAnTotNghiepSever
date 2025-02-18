using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class UserManager
    {
        public int CourseId { get; set; }
        public string? CourseName {  get; set; }
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
