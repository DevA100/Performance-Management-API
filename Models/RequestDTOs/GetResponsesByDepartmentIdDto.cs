namespace PerformanceSurvey.Models.DTOs
{
    public class GetResponsesByDepartmentIdDto
    {
        public int ResponseId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string ResponseText { get; set; }
        public int? OptionId { get; set; }
        public string Text { get; set; }
        public int? Score { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

