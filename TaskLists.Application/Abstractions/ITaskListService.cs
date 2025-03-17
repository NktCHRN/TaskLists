using FluentResults;
using TaskLists.Application.Common;
using TaskLists.Application.Features.TaskLists;

namespace TaskLists.Application.Abstractions;

public interface ITaskListService
{
    Task<Result<PagedDto<TaskListDto>>> GetTaskListsAsync(PaginationParametersQuery query);
    Task<Result<TaskListDetailedDto>> GetTaskListAsync(Guid id);
    Task<Result<Guid>> CreateTaskList(CreateTaskListCommand command);
    Task<Result> UpdateTaskList(Guid id, UpdateTaskListCommand command);
    Task<Result> DeleteTaskList(Guid id);
    Task<Result<IReadOnlyList<ConnectionDto>>> GetConnectionsAsync(Guid id);
    Task<Result> CreateConnection(Guid taskListId, Guid userId);
    Task<Result> DeleteConnection(Guid taskListId, Guid userId);
}
