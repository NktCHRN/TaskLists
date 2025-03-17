namespace TaskLists.Application.Common;

public sealed record PaginationParametersDto(int PageNumber, int PageSize, long TotalCount, long TotalPages);
