using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PerformanceSurvey.Models
{
    public class QuestionOption
    {
        [Key]
        public int OptionId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        
        public string Text { get; set; }

        public int Score { get; set; }

        [ForeignKey("QuestionId")]
        public Question DepartmentQuestion { get; set; }
    }
}
