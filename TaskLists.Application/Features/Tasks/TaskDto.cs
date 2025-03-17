namespace TaskLists.Application.Features.Tasks;

public sealed record TaskDto(Guid Id, string Name, DateTimeOffset? DueDate, bool IsCompleted)
{
    
}
