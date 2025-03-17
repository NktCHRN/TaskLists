using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TaskLists.Application.Abstractions;
using TaskLists.Application.Common;
using TaskLists.Application.Features.Tasks;
using TaskLists.WebApi.UserContext;

namespace TaskLists.WebApi.Controllers;

[UserContextRequired]
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IAspNetCoreResultEndpointProfile _resultEndpointProfile;

    public TasksController(ITaskService taskService, IAspNetCoreResultEndpointProfile resultEndpointProfile)
    {
        _taskService = taskService;
        _resultEndpointProfile = resultEndpointProfile;
    }

    [HttpGet("~/api/task-lists/{taskListId:guid}/tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedDto<TaskDto>>> GetTasks([FromRoute] Guid taskListId, [FromQuery] PaginationParametersQuery query)
    {
        var result = await _taskService.GetTasksByTaskList(taskListId, query);

        return result.ToActionResult(_resultEndpointProfile);
    }
    
    [HttpPost("~/api/task-lists/{taskListId:guid}/tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedDto<TaskDto>>> CreateTask([FromRoute] Guid taskListId, [FromBody] CreateTaskCommand command)
    {
        var result = await _taskService.CreateTask(taskListId, command);

        return result.ToActionResult(_resultEndpointProfile);
    }

    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTask([FromRoute] Guid taskId)
    {
        var result = await _taskService.DeleteTask(taskId);

        return result.ToActionResult(_resultEndpointProfile);
    }
}
