using Microsoft.Extensions.DependencyInjection;

namespace TaskLists.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services;
    }
}
