namespace TaskLists.Application.Common;

public sealed record PagedDto<TDto>(IReadOnlyCollection<TDto> Data, PaginationParametersDto PaginationParameters);
