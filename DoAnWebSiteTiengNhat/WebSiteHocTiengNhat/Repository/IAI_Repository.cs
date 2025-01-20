using WebSiteHocTiengNhat.Models;

namespace WebSiteHocTiengNhat.Repository
{
    public interface IAI_Repository
    {
      Task<string> SendMessage(string question, string Submission);
      Task<string> SendMessageAboutLesson(string question, int? lessonid);

    }
}
