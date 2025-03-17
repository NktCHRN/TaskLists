using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TaskLists.Application.Abstractions;
using TaskLists.Application.Common;
using TaskLists.Application.Features.Users;
using TaskLists.WebApi.Contracts.Common;

namespace TaskLists.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAspNetCoreResultEndpointProfile _resultEndpointProfile;

    public UsersController(IUserService userService, IAspNetCoreResultEndpointProfile resultEndpointProfile)
    {
        _userService = userService;
        _resultEndpointProfile = resultEndpointProfile;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedDto<UserDto>>>> GetUsers([FromQuery] PaginationParametersQuery query)
    {
        var users = await _userService.GetUsers(query);
        
        return users.ToActionResult(_resultEndpointProfile);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _userService.AddUser(command);
        
        return result.ToActionResult(_resultEndpointProfile);
    }
    
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser([FromRoute] Guid userId)
    {
        var user = await _userService.GetUser(userId);
        
        return user.ToActionResult(_resultEndpointProfile);
    }
}
