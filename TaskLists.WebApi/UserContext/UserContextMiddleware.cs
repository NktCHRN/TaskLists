using Microsoft.AspNetCore.Http.Features;
using TaskLists.Application.Abstractions;
using TaskLists.WebApi.Contracts.Common;

namespace TaskLists.WebApi.UserContext;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserContext userContext)
    {
        var valueFound = context.Request.Headers.TryGetValue("UserId", out var userIdValue);
        var parsed = Guid.TryParse(userIdValue.ToString(), out var userId);

        if (parsed)
        {
            ((WebApiUserContext)userContext).LoggedUserId = userId;
        }

        var hasAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata
            .Any(m => m is UserContextRequiredAttribute) is true;
        if (!parsed && hasAttribute)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var errorMessage = valueFound
                ? "\"UserId\" header is present, but the value is not a valid Guid."
                : "\"UserId\" header with valid Guid must be provided.";
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.FromError(
                    new ErrorResponse(errorMessage)));
            return;
        }
        
        await _next(context);
    }
}
