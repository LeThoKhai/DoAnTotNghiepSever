using WebSiteHocTiengNhat.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebSiteHocTiengNhat.ViewModels
{
    public class ExamAndQuestionExamViewModel
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public List<ExamQuestion> Examquestions { get; set; }
    }
}
