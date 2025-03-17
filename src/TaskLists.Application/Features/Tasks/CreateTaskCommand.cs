namespace TaskLists.Application.Features.Tasks;

public sealed record CreateTaskCommand(string? Name, string? Description, DateTimeOffset? DueDate)
{
    
}