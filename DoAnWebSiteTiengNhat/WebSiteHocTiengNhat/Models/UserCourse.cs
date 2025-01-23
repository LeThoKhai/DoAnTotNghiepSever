using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class UserCourse
    {
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        [JsonIgnore]
        public Course Course { get; set; }

        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }
        [JsonIgnore]
        public IdentityUser User { get; set; }
        public DateTime ExpirationTime { get; set; }
        public double progress { get; set; } = 0;
    }
}

