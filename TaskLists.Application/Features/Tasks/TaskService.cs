using FluentResults;
using TaskLists.Application.Abstractions;
using TaskLists.Application.Common;
using TaskLists.Domain.Abstractions;
using TaskLists.Domain.Errors;
using TaskLists.Domain.Models;
using TaskLists.Domain.Successes;

namespace TaskLists.Application.Features.Tasks;

public sealed class TaskService : ITaskService
{
    private readonly ITaskListRepository _taskListRepository;
    private readonly IUserContext _userContext;
    private readonly TimeProvider _timeProvider;
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskListRepository taskListRepository, IUserContext userContext, TimeProvider timeProvider, ITaskRepository taskRepository)
    {
        _taskListRepository = taskListRepository;
        _userContext = userContext;
        _timeProvider = timeProvider;
        _taskRepository = taskRepository;
    }

    public async Task<Result<PagedDto<TaskDto>>> GetTasksByTaskList(Guid taskListId, PaginationParametersQuery query)
    {
        var queryValidationResult = Pagination.ValidatePagination(query);
        if (queryValidationResult.IsFailed)
        {
            return queryValidationResult.ToResult<PagedDto<TaskDto>>();
        }
        
        var taskListResult = await GetTaskList(taskListId);
        if (!taskListResult.IsSuccess)
        {
            return taskListResult.ToResult<PagedDto<TaskDto>>();
        }

        var tasks = await _taskRepository.GetPaged(query.Page, query.PageSize, query.Ascending);
        
        var dto = new PagedDto<TaskDto>(
            tasks.Items.Select(u => new TaskDto(u.Id, u.Name, u.DueDate, u.IsCompleted)).ToList(),
            Pagination.GetPaginationParameters(tasks.Count, query));
        
        return Result.Ok(dto);
    }
    
    public async Task<Result<Guid>> CreateTask(Guid taskListId, CreateTaskCommand command)
    {
        var taskListResult = await GetTaskList(taskListId);
        if (!taskListResult.IsSuccess)
        {
            return taskListResult.ToResult<Guid>();
        }
        
        var taskResult = Domain.Models.Task.Create(command.Name, command.Description, command.DueDate, _timeProvider.GetUtcNow(), taskListId);
        if (!taskResult.IsSuccess)
        {
            return taskResult.ToResult<Guid>();
        }
        
        var creationResult = await _taskRepository.Create(taskResult.Value);
        return creationResult.Bind(() => Result.Ok<Guid>(taskResult.Value.Id).WithSuccess(new EntityCreatedSuccess()));
    }

    public async Task<Result> DeleteTask(Guid taskId)
    {
        var task = await _taskRepository.Get(taskId);
        if (task is null)
        {
            return Result.Fail(new NotFoundError("Task not found."));
        }
        
        var taskListResult = await GetTaskList(task.TaskListId);
        if (!taskListResult.IsSuccess)
        {
            return taskListResult.ToResult();
        }

        var deletionResult = await _taskRepository.Delete(task.Id);
        return deletionResult.Bind(() => Result.Ok().WithSuccess(new EntityDeletedSuccess()));
    }
    
    private async Task<Result<TaskList>> GetTaskList(Guid taskListId)
    {
        var taskList = await _taskListRepository.Get(taskListId);
        if (taskList is null)
        {
            return Result.Fail<TaskList>(new NotFoundError("TaskList not found"));
        }
        
        var connectionCheckResult = taskList.CheckConnection(_userContext.LoggedUserId.GetValueOrDefault());
        if (!connectionCheckResult.IsSuccess)
        {
            return connectionCheckResult.ToResult<TaskList>();
        }

        return Result.Ok();
    }

}
