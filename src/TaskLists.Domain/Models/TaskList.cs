using TaskLists.Domain.Abstractions;

namespace TaskLists.Domain.Models;

public sealed class TaskList : IEntity
{
    public Guid Id { get; private init; }
    public string Name { get; private set; } = string.Empty;
    
    public Guid OwnerId { get; private init; }

    public IReadOnlyList<Guid> ConnectedUsersIds => _connectedUsersIds;
    private readonly List<Guid> _connectedUsersIds = [];
    public DateTimeOffset CreationDateTime { get; init; }

    private TaskList(){}
    
    public TaskList(Guid id, string name, Guid ownerId, IReadOnlyList<Guid> connectedUsersIds, DateTimeOffset creationDateTime)
    {
        Id = id;
        Name = name;
        OwnerId = ownerId;
        _connectedUsersIds = connectedUsersIds.ToList();
        CreationDateTime = creationDateTime;
    }

    public static Result<TaskList> Create(string? name, Guid ownerId, DateTimeOffset creationDateTime)
    {
        var taskList = new TaskList
        {
            Id = Guid.CreateVersion7(),
            CreationDateTime = creationDateTime,
            OwnerId = ownerId,
        };

        var updateResult = taskList.Update(ownerId, name);
        return updateResult.Bind(() => Result.Ok(taskList).WithSuccess(new EntityCreatedSuccess()));
    }
    
    public Result Update(Guid loggedUserId, string? name)
    {
        var isLoggedUserConnected = CheckConnection(loggedUserId);
        if (isLoggedUserConnected.IsFailed)
        {
            return isLoggedUserConnected;
        }
        
        name = name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail(new ValidationError("Name is required."));
        }
        
        Name = name;
        return Result.Ok();
    }

    public Result AddConnection(Guid loggedUserId, Guid userId)
    {
        var isLoggedUserConnected = CheckConnection(loggedUserId);
        if (isLoggedUserConnected.IsFailed)
        {
            return isLoggedUserConnected;
        }

        if (OwnerId == userId)
        {
            return Result.Fail(new ConflictError("User is the owner of this task list."));
        }

        if (_connectedUsersIds.Contains(userId))
        {
            return Result.Fail(new ConflictError("User is already connected to this task list."));
        }
        
        _connectedUsersIds.Add(userId);
        return Result.Ok();
    }

    public Result RemoveConnection(Guid loggedUserId, Guid userId)
    {
        var isLoggedUserConnected = CheckConnection(loggedUserId);
        if (isLoggedUserConnected.IsFailed)
        {
            return isLoggedUserConnected;
        }
        
        if (OwnerId == userId)
        {
            return Result.Fail(new ForbiddenError("You cannot remove an owner."));
        }
        
        var removed = _connectedUsersIds.Remove(userId);
        
        return Result.OkIf(
            removed, 
            () => new NotFoundError("User is not connected to this task list."));
    }
    
    public Result CheckOwnership(Guid userId)
    {
        return Result.OkIf(
            OwnerId == userId, 
            () => new ForbiddenError("User is not an owner of this task list."));
    }
    
    public Result CheckConnection(Guid userId)
    {
        return Result.OkIf(
            OwnerId == userId || _connectedUsersIds.Contains(userId), 
            () => new ForbiddenError("User is not connected to this task list."));
    }
}
