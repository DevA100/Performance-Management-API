using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformanceSurvey.iServices;

namespace PerformanceSurvey.Controllers
{
    [Route("api/PerformanceSurvey")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Recepient) || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
            {
                return BadRequest("Email, subject, and body are required.");
            }

            try
            {
                await _emailService.SendEmailAsync(request.Recepient, request.Title, request.Body);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class EmailRequest
    {
        
        public string Recepient { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string? Attachments { get; set; }
    }
}
