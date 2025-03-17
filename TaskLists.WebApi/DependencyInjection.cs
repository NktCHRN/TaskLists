using FluentResults.Extensions.AspNetCore;
using TaskLists.Application.Abstractions;

namespace TaskLists.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddScoped<IUserContext, WebApiUserContext>()
            .AddSingleton<IAspNetCoreResultEndpointProfile, AspNetCoreResultEndpointProfile>();
    }
}
