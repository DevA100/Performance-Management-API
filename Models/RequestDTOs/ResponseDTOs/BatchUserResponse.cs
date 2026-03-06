namespace PerformanceSurvey.Models.RequestDTOs.ResponseDTOs
{
    public class BatchUserResponse
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public int SuccessCount { get; set; }
        public int TotalProcessed { get; set; }

        // Constructor - this is part of the main class
        public BatchUserResponse()
        {
            Errors = new List<string>();
            SuccessCount = 0;
            TotalProcessed = 0;
        }
    }
}
