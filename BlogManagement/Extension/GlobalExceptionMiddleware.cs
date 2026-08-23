using System.Text.Json;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;

namespace BlogManagement.Extension;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            int statusCode = ex switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                UnprocessableEntityException => StatusCodes.Status422UnprocessableEntity,
                InternalServerException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            await HandleExceptionAsync(context, statusCode, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse.ErrorResponse(statusCode, message);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
