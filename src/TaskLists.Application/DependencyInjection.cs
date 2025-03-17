using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskLists.Application.Abstractions;
using TaskLists.Application.Features.TaskLists;
using TaskLists.Application.Features.Tasks;
using TaskLists.Application.Features.Users;

namespace TaskLists.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddScoped<IUserService, UserService>()
            .AddScoped<ITaskListService, TaskListService>()
            .AddScoped<ITaskService, TaskService>();
    }
}
