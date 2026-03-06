namespace PerformanceSurvey.Middlewares
{
    public class RemoveSensitiveHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public RemoveSensitiveHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}