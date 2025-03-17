using MongoDB.Driver;
using TaskLists.Domain.Abstractions;
using TaskLists.Domain.Models;
using TaskLists.Infrastructure.Abstractions;

namespace TaskLists.Infrastructure.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IDatabaseFacade databaseFacade) : base(databaseFacade)
    {
    }

    public async Task<IReadOnlyList<User>> GetUsers(IEnumerable<Guid> userIds)
    {
        return await Collection.Find(u => userIds.Contains(u.Id)).ToListAsync();
    }
}
