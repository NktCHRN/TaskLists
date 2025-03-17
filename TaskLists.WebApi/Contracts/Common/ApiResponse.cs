namespace TaskLists.WebApi.Contracts.Common;

public sealed record ApiResponse<TData>(TData? Data, IEnumerable<ErrorResponse> Errors)
{
    public bool IsSuccess => !Errors.Any();
    
    public static ApiResponse<TData> Empty => new ApiResponse<TData>(default, []);

    public static ApiResponse<TData> FromData(TData? data)
    {
        return new ApiResponse<TData>(data, []);
    }

    public static ApiResponse<TData> FromErrors(IEnumerable<ErrorResponse> errors)
    {
        return new ApiResponse<TData>(default, errors);
    }

    public static ApiResponse<TData> FromError(ErrorResponse error)
    {
        return new ApiResponse<TData>(default, [error]);
    }
}
