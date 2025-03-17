namespace TaskLists.Application.Abstractions;

public interface IUserContext
{
    Guid? LoggedUserId { get; }
}
