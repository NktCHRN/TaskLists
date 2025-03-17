using FluentResults;

namespace TaskLists.Domain.Errors;

public sealed class NotFoundError(string message) : Error(message)
{
    
}