namespace TaskLists.Domain.Abstractions;

public interface ITaskRepository : IRepository<Task>
{
    Task<PagedEntities<Task>> GetByTaskListIdPaged(Guid taskListId, int pageNumber, int pageSize, bool isAscending = true);
    System.Threading.Tasks.Task DeleteByTaskListId(Guid taskListId);
}
