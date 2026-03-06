using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerformanceSurvey.Models
{
    public class AssignmentQuestion
    {
        [Key]
        public int AssignmentQuestionId { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int status { get; set; }
        public DateTime AssignedAt { get; set; }


    }
}
