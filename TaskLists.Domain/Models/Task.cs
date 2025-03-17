using TaskLists.Domain.Abstractions;

namespace TaskLists.Domain.Models;

public sealed class Task : IEntity
{
    public Guid Id { get; private init; }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public DateTimeOffset CreationDateTime { get; private init; }
    public bool IsCompleted { get; private set; }
    
    public Guid TaskListId { get; private init; }
    
    private Task() { }

    public Task(Guid id, string name, string? description, DateTimeOffset? dueDate, DateTimeOffset creationDateTime, bool isCompleted, Guid taskListId)
    {
        Id = id;
        Name = name;
        Description = description;
        DueDate = dueDate;
        CreationDateTime = creationDateTime;
        IsCompleted = isCompleted;
        TaskListId = taskListId;
    }

    public static Result<Task> Create(string? name, string? description, DateTimeOffset? dueDate, DateTimeOffset creationDateTime, Guid taskListId)
    {
        var task = new Task
        {
            Id = Guid.CreateVersion7(),
            CreationDateTime = creationDateTime,
            TaskListId = taskListId,
        };

        var updateResult = task.Update(name, description, dueDate);
        return updateResult.Bind(() => Result.Ok(task).WithSuccess(new EntityCreatedSuccess()));
    }

    public Result Update(string? name, string? description, DateTimeOffset? dueDate)
    {
        name = name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail(new ValidationError("Name is required."));
        }
        
        Name = name;
        Description = description;
        DueDate = dueDate;
        
        return Result.Ok();
    }

    public Result ChangeStatus(bool isCompleted)
    {
        IsCompleted = isCompleted;
        return Result.Ok();
    }
}
