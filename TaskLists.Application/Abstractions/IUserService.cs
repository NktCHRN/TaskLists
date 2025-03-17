using FluentResults;
using TaskLists.Application.Common;
using TaskLists.Application.Features.Users;

namespace TaskLists.Application.Abstractions;

public interface IUserService
{
    Task<Result<PagedDto<UserDto>>> GetUsers(PaginationParametersQuery query);
    Task<Result<Guid>> AddUser(CreateUserCommand command);
    Task<Result<UserDto>> GetUser(Guid id);
}
