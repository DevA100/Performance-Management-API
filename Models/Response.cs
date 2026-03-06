using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PerformanceSurvey.Models
{
    public class Response
    {
        [Key]
        public int ResponseId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        public string? ResponseText { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

       
        public int? OptionId { get; set; }

        [ForeignKey("OptionId")]
        public QuestionOption? QuestionOption { get; set; }

        public int? Score { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
    public enum MultipleChoiceScore
    {
        Very_Poor,
        Poor,
        Good,
        Very_Good,
        Excellent

    }
}
