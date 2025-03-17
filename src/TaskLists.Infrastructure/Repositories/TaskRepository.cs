using FluentResults;
using MongoDB.Driver;
using TaskLists.Domain.Abstractions;
using TaskLists.Infrastructure.Abstractions;

namespace TaskLists.Infrastructure.Repositories;

public sealed class TaskRepository : Repository<Domain.Models.Task>, ITaskRepository
{
    public TaskRepository(IDatabaseFacade databaseFacade) : base(databaseFacade)
    {
    }

    public async Task<PagedEntities<Domain.Models.Task>> GetByTaskListIdPaged(Guid taskListId, int pageNumber, int pageSize, bool isAscending = true)
    {
        IFindFluent<Domain.Models.Task, Domain.Models.Task> query = Collection.Find(x => x.TaskListId == taskListId);
        
        var count = await query.CountDocumentsAsync();
        
        var entities = await GetPagedItems(
            query,
            pageNumber,
            pageSize,
            isAscending);
        
        return new PagedEntities<Domain.Models.Task>(entities, count);
    }

    public async Task DeleteByTaskListId(Guid taskListId)
    {
        await Collection.DeleteManyAsync(x => x.TaskListId == taskListId);
    }
}
