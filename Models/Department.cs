using System.ComponentModel.DataAnnotations;

namespace PerformanceSurvey.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(250)]
        public string DepartmentName { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDisabled { get; set; }
    }
}
