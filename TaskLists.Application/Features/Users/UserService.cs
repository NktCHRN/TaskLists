using FluentResults;
using TaskLists.Application.Abstractions;
using TaskLists.Application.Common;
using TaskLists.Domain.Abstractions;
using TaskLists.Domain.Errors;
using TaskLists.Domain.Models;
using TaskLists.Domain.Successes;

namespace TaskLists.Application.Features.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository  _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PagedDto<UserDto>>> GetUsers(PaginationParametersQuery query)
    {
        var queryValidationResult = Pagination.ValidatePagination(query);
        if (queryValidationResult.IsFailed)
        {
            return queryValidationResult.ToResult<PagedDto<UserDto>>();
        }
        
        var users = await _userRepository.GetPaged(query.Page, query.PageSize, query.Ascending);

        var dto = new PagedDto<UserDto>(
            users.Items.Select(u => new UserDto(u.Id, u.Name)).ToList(),
            Pagination.GetPaginationParameters(users.Count, query));
        
        return Result.Ok(dto);
    }

    public async Task<Result<Guid>> AddUser(CreateUserCommand command)
    {
        var userResult = User.Create(command.Name);
        if (!userResult.IsSuccess)
        {
            return userResult.ToResult<Guid>();
        }

        var user = userResult.Value;
        var createResult = await _userRepository.Create(user);
        
        return createResult.Bind(() => Result.Ok(user.Id).WithSuccess(new EntityCreatedSuccess()));
    }

    public async Task<Result<UserDto>> GetUser(Guid id)
    {
        var user = await _userRepository.Get(id);

        return user is not null 
            ? Result.Ok(new UserDto(user.Id, user.Name)) 
            : Result.Fail(new NotFoundError("User was not found"));
    }
}
