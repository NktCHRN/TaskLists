using MongoDB.Driver;
using TaskLists.Domain.Models;

namespace TaskLists.Infrastructure.Abstractions;

public interface IDatabaseFacade
{
    public IMongoCollection<TaskList> TaskLists { get; }
    public IMongoCollection<Domain.Models.Task> Tasks { get; }
    public IMongoCollection<User> Users { get; }
    IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : class;

    System.Threading.Tasks.Task SetupDatabase();
}
