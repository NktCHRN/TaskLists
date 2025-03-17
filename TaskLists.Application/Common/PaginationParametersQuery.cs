namespace TaskLists.Application.Common;

public record PaginationParametersQuery(int Page = 1, int PageSize = 10, bool Ascending = true);
