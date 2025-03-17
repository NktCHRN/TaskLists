using FluentResults;
using TaskLists.Application.Abstractions;
using TaskLists.Application.Common;
using TaskLists.Domain.Abstractions;
using TaskLists.Domain.Errors;
using TaskLists.Domain.Models;
using TaskLists.Domain.Successes;

namespace TaskLists.Application.Features.TaskLists;

public sealed class TaskListService : ITaskListService
{
    private readonly ITaskListRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ITaskRepository _taskRepository;

    public TaskListService(ITaskListRepository repository, IUserContext userContext, IUserRepository userRepository, TimeProvider timeProvider, ITaskRepository taskRepository)
    {
        _repository = repository;
        _userContext = userContext;
        _userRepository = userRepository;
        _timeProvider = timeProvider;
        _taskRepository = taskRepository;
    }

    public async Task<Result<PagedDto<TaskListDto>>> GetTaskListsAsync(PaginationParametersQuery query)
    {
        var queryValidationResult = Pagination.ValidatePagination(query);
        if (!queryValidationResult.IsSuccess)
        {
            return queryValidationResult.ToResult<PagedDto<TaskListDto>>();
        }

        var currentUserId = _userContext.LoggedUserId.GetValueOrDefault();
        
        var taskLists = await _repository.GetByUserIdPaged(currentUserId, query.Page, query.PageSize, query.Ascending);
        
        var dto = new PagedDto<TaskListDto>(
            taskLists.Items.Select(u => new TaskListDto(u.Id, u.Name)).ToList(),
            Pagination.GetPaginationParameters(taskLists.Count, query));
        
        return Result.Ok(dto);
    }

    public async Task<Result<TaskListDetailedDto>> GetTaskListAsync(Guid id)
    {
        var taskList = await _repository.Get(id);
        if (taskList is null)
        {
            return Result.Fail<TaskListDetailedDto>(new NotFoundError("TaskList not found"));
        }

        var connectionCheckResult = taskList.CheckConnection(_userContext.LoggedUserId.GetValueOrDefault());
        if (!connectionCheckResult.IsSuccess)
        {
            return connectionCheckResult.ToResult<TaskListDetailedDto>();
        }
        
        var owner = await _userRepository.Get(taskList.OwnerId);
        if (owner is null)
        {
            return Result.Fail<TaskListDetailedDto>(new NotFoundError("User not found"));
        }

        return Result.Ok(
            new TaskListDetailedDto(taskList.Id, taskList.Name, taskList.OwnerId, owner.Name, taskList.CreationDateTime));
    }

    public async Task<Result<Guid>> CreateTaskList(CreateTaskListCommand command)
    {
        var validationResult = TaskListValidator.Validate(command);
        if (!validationResult.IsSuccess)
        {
            return validationResult.ToResult<Guid>();
        }

        var currentUser = await _userRepository.Get(_userContext.LoggedUserId.GetValueOrDefault());
        if (currentUser == null)
        {
            return Result.Fail<Guid>(new NotFoundError("User not found"));
        }

        var taskListResult = TaskList.Create(command.Name, _userContext.LoggedUserId.GetValueOrDefault(), _timeProvider.GetUtcNow());
        if (!taskListResult.IsSuccess)
        {
            return taskListResult.ToResult<Guid>();
        }
        
        var creationResult = await _repository.Create(taskListResult.Value);
        return creationResult.Bind(() => Result.Ok(taskListResult.Value.Id).WithSuccess(new EntityCreatedSuccess()));
    }

    public async Task<Result> UpdateTaskList(Guid id, UpdateTaskListCommand command)
    {
        var validationResult = TaskListValidator.Validate(command);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        
        var taskList = await _repository.Get(id);
        if (taskList is null)
        {
            return Result.Fail(new NotFoundError("TaskList not found"));
        }
        
        var taskListResult = taskList.Update(_userContext.LoggedUserId.GetValueOrDefault(), command.Name);
        if (!taskListResult.IsSuccess)
        {
            return taskListResult;
        }

        var updateResult = await _repository.Update(taskList);
        return updateResult;
    }

    public async Task<Result> DeleteTaskList(Guid id)
    {
        var taskList = await _repository.Get(id);
        if (taskList is null)
        {
            return Result.Fail(new NotFoundError("TaskList not found"));
        }
        
        var taskOwnershipResult = taskList.CheckOwnership(_userContext.LoggedUserId.GetValueOrDefault());
        if (!taskOwnershipResult.IsSuccess)
        {
            return taskOwnershipResult;
        }
        
        await _taskRepository.DeleteByTaskListId(id);
        var deletionResult = await _repository.Delete(id);

        return deletionResult.Bind(() => Result.Ok().WithSuccess(new EntityDeletedSuccess()));
    }

    public async Task<Result<IReadOnlyList<ConnectionDto>>> GetConnectionsAsync(Guid id)
    {
        var taskList = await _repository.Get(id);
        if (taskList is null)
        {
            return Result.Fail<IReadOnlyList<ConnectionDto>>(new NotFoundError("TaskList not found"));
        }

        var connectionCheckResult = taskList.CheckConnection(_userContext.LoggedUserId.GetValueOrDefault());
        if (!connectionCheckResult.IsSuccess)
        {
            return connectionCheckResult.ToResult<IReadOnlyList<ConnectionDto>>();
        }
        
        var users = (await _userRepository.GetUsers(taskList.ConnectedUsersIds))
            .ToDictionary(u => u.Id, u => u.Name);

        return Result.Ok(
            (IReadOnlyList<ConnectionDto>)taskList.ConnectedUsersIds
                .Select(c => new ConnectionDto(c, users[c]))
                .ToList());
    }

    public async Task<Result> CreateConnection(Guid taskListId, Guid userId)
    {
        var taskList = await _repository.Get(taskListId);
        if (taskList is null)
        {
            return Result.Fail(new NotFoundError("TaskList not found"));
        }
        
        var taskListResult = taskList.AddConnection(_userContext.LoggedUserId.GetValueOrDefault(), userId);
        if (!taskListResult.IsSuccess)
        {
            return taskListResult;
        }
        
        var updateResult = await _repository.Update(taskList);
        return updateResult.Bind(() => Result.Ok().WithSuccess(new EntityCreatedSuccess()));
    }

    public async Task<Result> DeleteConnection(Guid taskListId, Guid userId)
    {
        var taskList = await _repository.Get(taskListId);
        if (taskList is null)
        {
            return Result.Fail(new NotFoundError("TaskList not found"));
        }
        
        var taskListResult = taskList.RemoveConnection(_userContext.LoggedUserId.GetValueOrDefault(), userId);
        if (!taskListResult.IsSuccess)
        {
            return taskListResult;
        }
        
        var updateResult = await _repository.Update(taskList);
        return updateResult.Bind(() => Result.Ok().WithSuccess(new EntityDeletedSuccess()));
    }
}