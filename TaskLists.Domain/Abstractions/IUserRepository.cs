using TaskLists.Domain.Models;

namespace TaskLists.Domain.Abstractions;

public interface IUserRepository : IRepository<User>
{
    Task<IReadOnlyList<User>> GetUsers(IEnumerable<Guid> userIds);
}
