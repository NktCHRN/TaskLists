using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TaskLists.Application.Abstractions;
using TaskLists.Application.Common;
using TaskLists.Application.Features.TaskLists;
using TaskLists.Application.Features.Users;
using TaskLists.WebApi.Contracts.Common;
using TaskLists.WebApi.UserContext;

namespace TaskLists.WebApi.Controllers;

[UserContextRequired]
[ApiController]
[Route("api/[controller]")]
public sealed class TaskListsController : ControllerBase
{
    private readonly ITaskListService _taskListService;
    private readonly IAspNetCoreResultEndpointProfile _resultEndpointProfile;
    
    public TaskListsController(ITaskListService taskListService, IAspNetCoreResultEndpointProfile resultEndpointProfile)
    {
        _taskListService = taskListService;
        _resultEndpointProfile = resultEndpointProfile;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedDto<UserDto>>>> GetTaskLists([FromQuery] PaginationParametersQuery query)
    {
        var taskLists = await _taskListService.GetTaskListsAsync(query);
        
        return taskLists.ToActionResult(_resultEndpointProfile);
    }
    
    [HttpGet("{taskListId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetTaskList([FromRoute] Guid taskListId)
    {
        var taskList = await _taskListService.GetTaskListAsync(taskListId);
        
        return taskList.ToActionResult(_resultEndpointProfile);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateTaskList([FromBody] CreateTaskListCommand command)
    {
        var result = await _taskListService.CreateTaskList(command);
        
        return result.ToActionResult(_resultEndpointProfile);
    }
    
    [HttpPut("{taskListId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateTaskList([FromRoute] Guid taskListId, [FromBody] UpdateTaskListCommand command)
    {
        var result = await _taskListService.UpdateTaskList(taskListId, command);
        
        return result.ToActionResult(_resultEndpointProfile);
    }
    
    [HttpDelete("{taskListId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTaskList([FromRoute] Guid taskListId)
    {
        var result = await _taskListService.DeleteTaskList(taskListId);
        
        return result.ToActionResult(_resultEndpointProfile);
    }

    [HttpGet("{taskListId:guid}/connections")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<ConnectionDto>>> GetConnections([FromRoute] Guid taskListId)
    {
        var result = await _taskListService.GetConnectionsAsync(taskListId);
        
        return result.ToActionResult(_resultEndpointProfile);
    }

    [HttpPost("{taskListId:guid}/connections/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateConnection(Guid taskListId, Guid userId)
    {
        var result = await _taskListService.CreateConnection(taskListId, userId);
        
        return result.ToActionResult(_resultEndpointProfile);
    }
    
    [HttpDelete("{taskListId:guid}/connections/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteConnection(Guid taskListId, Guid userId)
    {
        var result = await _taskListService.DeleteConnection(taskListId, userId);
        
        return result.ToActionResult(_resultEndpointProfile);
    }
}
