using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TaskLists.Domain.Abstractions;
using TaskLists.Infrastructure.Abstractions;
using TaskLists.Infrastructure.Repositories;

namespace TaskLists.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("MongoDb");
        var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
        services.AddSingleton<IMongoClient>(new MongoClient(mongoClientSettings)); 
        
        DatabaseFacade.RegisterSettings();
        services.AddSingleton<IDatabaseFacade, DatabaseFacade>();

        services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();

        services.AddSingleton<ITaskListRepository, TaskListRepository>();
        services.AddSingleton<ITaskRepository, TaskRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();

        services.AddSingleton(TimeProvider.System);
        
        return services;
    }
}
