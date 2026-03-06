namespace PerformanceSurvey.iServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string Recepient, string Title, string body);
    }
}
