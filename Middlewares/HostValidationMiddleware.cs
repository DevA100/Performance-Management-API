
namespace PerformanceSurvey.Middlewares
{
    public class HostValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedHosts;
        private readonly ILogger<HostValidationMiddleware> _logger;

        public HostValidationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<HostValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            var allowedHostsString = configuration["AllowedHosts"];
            _allowedHosts = allowedHostsString?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

            _logger.LogInformation("Host validation middleware initialized with hosts: {AllowedHosts}",
                string.Join(", ", _allowedHosts));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestHost = context.Request.Host.Host;

            if (!_allowedHosts.Contains(requestHost, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid host header rejected: {RequestHost}", requestHost);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid Host Header");
                return;
            }

            await _next(context);
        }
    }
}
