using TaskLists.WebApi.Contracts.Common;

namespace TaskLists.WebApi;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            var errorId = Guid.NewGuid();
            _logger.LogError(
                exception, "Exception occurred (id: {ErrorId}): {ErrorMessage}", errorId, exception.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;     // User errors should be handled via Results.

            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.FromError(new ErrorResponse($"An unexpected error occurred (error id: {errorId}).")));
        }

    }
}
