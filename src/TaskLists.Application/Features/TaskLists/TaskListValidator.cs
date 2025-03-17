using FluentResults;
using TaskLists.Domain.Errors;

namespace TaskLists.Application.Features.TaskLists;

public static class TaskListValidator
{
    public static Result Validate(CreateTaskListCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return ValidateName(command.Name);
    }
    
    public static Result Validate(UpdateTaskListCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return ValidateName(command.Name);
    }
    
    public static Result ValidateName(string? name)
    {
        if (name is null)
        {
            return Result.Fail(new ValidationError("Name is required"));
        }

        if (name.Length is < 1 or > 255)
        {
            return Result.Fail(new ValidationError("Name must be between 1 and 255 characters long"));
        }
        
        return Result.Ok();
    }
}
