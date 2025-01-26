using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebSiteHocTiengNhat.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
       // [Required]
        public string QuestionText { get; set; }
        //[Required]
        public string? Link { get; set; }

        //[Required]
        public string? OptionA { get; set; }
       // [Required]
        public string? OptionB { get; set; }

        //[Required]
        public string? OptionC { get; set; }

       // [Required]
        public string? OptionD { get; set; }

        //[Required]
        public string CorrectAnswer { get; set; }
        //[Required]
        public string? CorrectAnswerString { get; set; }

        [Required]
        public int Level { get; set; } = 1;
        [Required]
        [ForeignKey("CategoryQuestion")]
        public int CategoryQuestionId { get; set; }
        [JsonIgnore]
        public CategoryQuestion? CategoryQuestion { get; set; }


        [Required]
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }
        [JsonIgnore]
        public Exercise? Exercise { get; set; }
        [Required]
        [ForeignKey("QuestionType")]
        public string QuestionTypeId { get; set; }
        [JsonIgnore]
        public QuestionType? QuestionType{ get; set; }

        //questiontype

        /*      public bool FillInBlanks { get; set; } = false; // tra ve diem, dien tu vao o trong
                public bool ReOrderParagraphs { get; set; } = false; // tra ve diem, sap xep cau
                public bool McSingleAnswer { get; set; } = false; // tra ve diem, tra loi 1 dap an
                public bool McMultipleAnswer { get; set; } = false; // tra ve diem, tra loi nhieu dap an
                public bool WriteFromDictation { get; set; } = false;// tra ve diem, viet lai doan van vua nghe
                public bool AnalysisText { get; set; } = false;// tra ve diem, loi khuyen AI, tom tat doan van
                public bool WriteEssay { get; set; } = false;// tra ve diem, loi khuyen AI, viet doan van dua tren de bai
                public bool RepeatSentence { get; set; } = false;// tra ve diem, loi khuyen AI, noi lap lai
                public bool DescribeImage { get; set; } = false;// tra ve diem, dung AI de phan tich, mieu ta hinh anh
                public bool AnswerShortQuestion { get; set; } = false;// dung AI de phan tich, tra loi cau hoi*/
    }
}
