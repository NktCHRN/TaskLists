using FluentResults;
using TaskLists.Domain.Abstractions;
using TaskLists.Domain.Errors;

namespace TaskLists.Application.Common;

public static class Pagination
{
    public static long GetTotalPagesCount(int pageSize, long totalCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageSize, 0);
        
        if (totalCount == 0)
        {
            return 1;
        }
        
        return (long)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static Result ValidatePagination(PaginationParametersQuery query)
    {
        if (query.PageSize <= 0)
        {
            return Result.Fail(new ValidationError("Page size must be greater than zero"));
        }
        
        if (query.Page <= 0)
        {
            return Result.Fail(new ValidationError("Page index must be greater than zero"));
        }
        
        return Result.Ok();
    }

    public static PaginationParametersDto GetPaginationParameters(long entitiesCount,
        PaginationParametersQuery query)
    {
        return new PaginationParametersDto(
            query.Page,
            query.PageSize,
            entitiesCount,
            Pagination.GetTotalPagesCount(query.PageSize, entitiesCount));
    }
}
