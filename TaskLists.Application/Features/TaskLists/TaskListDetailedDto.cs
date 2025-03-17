namespace TaskLists.Application.Features.TaskLists;

public sealed record TaskListDetailedDto(Guid Id, string Name, Guid OwnerId, string OwnerName, DateTimeOffset CreationDateTime)
{
    
}
