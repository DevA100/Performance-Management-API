namespace PerformanceSurvey.Models.DTOs
{
    public class CreateAdminUserRequest
    {
    public string Name { get; set; }
    public string UserEmail { get; set; }
    public string Password { get; set; } 
    public string Key { get; set; }
    }
}
