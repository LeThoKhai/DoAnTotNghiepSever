using System.ComponentModel.DataAnnotations;

namespace WebSiteHocTiengNhat.Models
{
    public class CategoryQuestion
    {
        [Key]
        public int CategoryQuestionId { get; set; }

        [Required]
        public string CategoryQuestionName { get; set; }
        
        public bool IsListening { get; set; } = false;
        public bool IsReading { get; set; } = false;
        public bool IsGrammarVocabulary { get; set; } = false;
        public bool IsSpeacking { get; set; } = false;
        public bool IsWriting { get; set; } = false;


    }
}
