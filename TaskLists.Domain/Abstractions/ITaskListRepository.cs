using TaskLists.Domain.Models;

namespace TaskLists.Domain.Abstractions;

public interface ITaskListRepository : IRepository<TaskList>
{
    Task<PagedEntities<TaskList>> GetByUserIdPaged(Guid userId, int pageNumber, int pageSize, bool isAscending = true);
}
