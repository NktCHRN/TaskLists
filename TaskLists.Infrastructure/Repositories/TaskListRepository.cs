using MongoDB.Driver;
using TaskLists.Domain.Abstractions;
using TaskLists.Domain.Models;
using TaskLists.Infrastructure.Abstractions;

namespace TaskLists.Infrastructure.Repositories;

public sealed class TaskListRepository : Repository<TaskList>, ITaskListRepository
{
    public TaskListRepository(IDatabaseFacade databaseFacade) : base(databaseFacade)
    {
    }

    public async Task<PagedEntities<TaskList>> GetByUserIdPaged(Guid userId, int pageNumber, int pageSize, bool isAscending = true)
    {
        IFindFluent<TaskList, TaskList> query = Collection.Find(x =>
            x.OwnerId == userId || x.ConnectedUsersIds.Contains(userId));        // Add index in future migrations.
        
        var count = await query.CountDocumentsAsync();
        
        var entities = await GetPagedItems(
            query,
            pageNumber,
            pageSize,
            isAscending);
        
        return new PagedEntities<TaskList>(entities, count);
    }
}
