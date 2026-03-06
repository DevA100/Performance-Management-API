namespace PerformanceSurvey.Models.DTOs
{
    public class AssignDiffQuestionsToDepartmentDto
    {
        public List<int> SourceDepartmentId { get; set; }

        public int TargetDepartmentId { get; set; }

    }
}
