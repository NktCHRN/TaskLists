using FluentResults;

namespace TaskLists.Domain.Errors;

public sealed class ConflictError(string message) : Error(message)
{
}