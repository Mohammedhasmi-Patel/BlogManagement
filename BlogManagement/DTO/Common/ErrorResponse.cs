namespace BlogManagement.DTO.Common;

public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;

    public ErrorResponse(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }
}
