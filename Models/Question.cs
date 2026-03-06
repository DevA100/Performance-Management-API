using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PerformanceSurvey.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(500)] 
        public string QuestionText { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public ICollection<QuestionOption>? Options { get; set; } = new List<QuestionOption>();

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDisabled { get; set; }
    }
}
