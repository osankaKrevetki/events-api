using EventsApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventsApi.Middleware;

/// <summary>
/// Глобальный обработчик исключений: перехватывает все необработанные ошибки pipeline
/// и возвращает единообразный ответ в формате Problem Details (RFC 7807).
/// Типы ошибок: ValidationException → 400, NotFoundException → 404, остальное → 500.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Ошибка валидации"),
            NotFoundException => (StatusCodes.Status404NotFound, "Ресурс не найден"),
            _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Необработанное исключение при обработке запроса {Path}",
                context.Request.Path);
        }
        else
        {
            _logger.LogWarning("{ExceptionType} при обработке запроса {Path}: {Message}",
                exception.GetType().Name, context.Request.Path, exception.Message);
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
    }
}
