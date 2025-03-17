using FluentResults;

namespace TaskLists.Domain.Errors;

public sealed class ValidationError(string message) : Error(message)
{
    
}
