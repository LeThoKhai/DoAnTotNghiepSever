using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class UserExerciseViewModel
    {
        public Course? course { get; set; }
        public Exercise? exercise { get; set; }
        public Category? category{ get; set; }
        public List<Question>? questions { get; set; } = new List<Question>();
        public double? Score { get; set; } 
        public bool? Answer {  get; set; } = false;
    }
}
