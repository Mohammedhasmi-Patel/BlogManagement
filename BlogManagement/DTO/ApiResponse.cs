namespace BlogManagement.DTO;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, int statusCode, string message, T? data = default)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public static ApiResponse<T> SuccessResponse(T? data, int statusCode = 200, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(int statusCode, string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = default
        };
    }
}

public static class ApiResponse
{
    public static ApiResponse<T> SuccessResponse<T>(T? data, int statusCode = 200, string message = "Success")
    {
        return ApiResponse<T>.SuccessResponse(data, statusCode, message);
    }

    public static ApiResponse<object> ErrorResponse(int statusCode, string message)
    {
        return ApiResponse<object>.ErrorResponse(statusCode, message);
    }
}
