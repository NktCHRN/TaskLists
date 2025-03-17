using TaskLists.Application.Abstractions;

namespace TaskLists.WebApi;

public sealed class WebApiUserContext : IUserContext
{
    public Guid? LoggedUserId { get; internal set; }
}
