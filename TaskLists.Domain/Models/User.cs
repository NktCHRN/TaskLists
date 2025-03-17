using TaskLists.Domain.Abstractions;

namespace TaskLists.Domain.Models;

public sealed class User : IEntity
{
    public Guid Id { get; private init; }
    public string Name { get; private set; }  = string.Empty;

    private User() {}

    public User(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Result<User> Create(string? name)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7()
        };

        var updateResult = user.Update(name);
        return updateResult.Bind(() => Result.Ok(user).WithSuccess(new EntityCreatedSuccess()));
    }

    public Result Update(string? name)
    {
        name = name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail(new ValidationError("Name is required."));
        }
        
        Name = name;
        return Result.Ok();
    }
}
