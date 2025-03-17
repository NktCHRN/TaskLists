using FluentResults;

namespace TaskLists.Domain.Errors;

public sealed class ForbiddenError(string message) : Error(message);
