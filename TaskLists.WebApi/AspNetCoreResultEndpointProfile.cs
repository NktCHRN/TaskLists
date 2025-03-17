using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TaskLists.Domain.Errors;
using TaskLists.Domain.Successes;
using TaskLists.WebApi.Contracts.Common;

namespace TaskLists.WebApi;

public sealed class AspNetCoreResultEndpointProfile : IAspNetCoreResultEndpointProfile
{
    private readonly ILogger<AspNetCoreResultEndpointProfile> _logger;

    public AspNetCoreResultEndpointProfile(ILogger<AspNetCoreResultEndpointProfile> logger)
    {
        _logger = logger;
    }

    public ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        var result = context.Result;

        var errorsToShow = result.Errors.Select(e =>
        {
            var errorMessage = e.Message;

            if (e is UnexpectedError)
            {
                var errorId = Guid.NewGuid();
                _logger.LogError("Exception occurred (id: {ErrorId}): {ErrorMessage}", errorId, e.Message);

                errorMessage = $"An unexpected error occurred (error id: {errorId}).";
            }
            
            return new ErrorResponse(errorMessage);
        })
        .ToList();
        var responseObject = ApiResponse<object>.FromErrors(errorsToShow);

        if (result.Errors.Any(e => e is UnexpectedError))
        {
            return new ObjectResult(responseObject)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        
        var firstError = result.Errors[0];
        ActionResult actionResult = firstError switch
        {
            ValidationError => new UnprocessableEntityObjectResult(responseObject),
            NotFoundError => new NotFoundObjectResult(responseObject),
            ForbiddenError => new ObjectResult(responseObject)
            {
                StatusCode = StatusCodes.Status403Forbidden
            },
            ConflictError => new ConflictObjectResult(responseObject),
            _ => new ObjectResult(responseObject)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };
        
        return actionResult;
    }

    public ActionResult TransformOkNoValueResultToActionResult(OkResultToActionResultTransformationContext<Result> context)
    {
        var firstSuccess = context.Result.Successes.FirstOrDefault();

        ActionResult actionResult = firstSuccess switch
        {
            EntityDeletedSuccess => new NoContentResult(),
            _ => new OkObjectResult(ApiResponse<object>.Empty)
        };
        
        return actionResult;
    }

    public ActionResult TransformOkValueResultToActionResult<T>(OkResultToActionResultTransformationContext<Result<T>> context)
    {
        var firstSuccess = context.Result.Successes.FirstOrDefault();

        var responseObject = ApiResponse<T>.FromData(context.Result.Value);
        ActionResult actionResult = firstSuccess switch
        {
            EntityCreatedSuccess => new ObjectResult(responseObject)
            {
                StatusCode = StatusCodes.Status201Created
            }, 
            _ => new OkObjectResult(responseObject)
        };
        
        return actionResult;
    }
}
