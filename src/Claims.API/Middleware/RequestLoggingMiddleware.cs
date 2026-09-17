using System.Diagnostics;

namespace Claims.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // The raw method and path can contain attacker-controlled characters.
            // Keep request logs useful without copying either value into a log entry.
            var method = context.Request.Method switch
            {
                "GET" => "GET",
                "POST" => "POST",
                "PUT" => "PUT",
                "PATCH" => "PATCH",
                "DELETE" => "DELETE",
                "HEAD" => "HEAD",
                "OPTIONS" => "OPTIONS",
                _ => "OTHER"
            };
            var endpoint = context.GetEndpoint()?.DisplayName ?? "Unmatched endpoint";

            _logger.LogInformation(
                "HTTP {Method} {Endpoint} responded {StatusCode} in {ElapsedMilliseconds} ms",
                method,
                endpoint,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
