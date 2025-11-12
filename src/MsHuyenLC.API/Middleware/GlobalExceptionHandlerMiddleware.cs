using System.Net;
using System.Text.Json;
using FluentValidation;

namespace MsHuyenLC.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ErrorResponse
        {
            Success = false
        };

        switch (exception)
        {
            case ValidationException fluentValidationException:
                _logger.LogWarning(fluentValidationException, "FluentValidation error occurred");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Dữ liệu không hợp lệ";
                response.Errors = fluentValidationException.Errors.Select(e => new ErrorDetail
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();
                break;

            case KeyNotFoundException:
                _logger.LogWarning(exception, "Resource not found");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = exception.Message ?? "Không tìm thấy tài nguyên";
                break;

            case UnauthorizedAccessException:
                _logger.LogWarning(exception, "Unauthorized access attempt");
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.Message = "Bạn không có quyền truy cập tài nguyên này";
                break;

            case InvalidOperationException:
                _logger.LogWarning(exception, "Invalid operation");
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = exception.Message ?? "Thao tác không hợp lệ";
                break;

            case ArgumentNullException:
            case ArgumentException:
                _logger.LogWarning(exception, "Bad request");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = exception.Message ?? "Tham số không hợp lệ";
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";
                break;
        }

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(result);
    }
}

public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ErrorDetail>? Errors { get; set; }
}

public class ErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
