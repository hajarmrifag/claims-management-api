namespace Claims.API.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteErrorResponse(
                context,
                StatusCodes.Status400BadRequest,
                ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteErrorResponse(
                context,
                StatusCodes.Status401Unauthorized,
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteErrorResponse(
                context,
                StatusCodes.Status409Conflict,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");

            await WriteErrorResponse(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            error = message
        });
    }
}
