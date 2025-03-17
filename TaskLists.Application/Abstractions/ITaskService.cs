using FluentResults;
using TaskLists.Application.Common;
using TaskLists.Application.Features.Tasks;

namespace TaskLists.Application.Abstractions;

public interface ITaskService
{
    Task<Result<PagedDto<TaskDto>>> GetTasksByTaskList(Guid taskListId, PaginationParametersQuery query);
    Task<Result<Guid>> CreateTask(Guid taskListId, CreateTaskCommand command);
    Task<Result> DeleteTask(Guid taskId);
}
