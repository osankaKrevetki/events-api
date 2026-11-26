using EventsApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventsApi.Middleware;

/// <summary>
/// Глобальный обработчик исключений: перехватывает все необработанные ошибки pipeline
/// и возвращает единообразный ответ в формате Problem Details (RFC 7807).
/// ValidationException и BadHttpRequestException → 400, NotFoundException → 404, остальное → 500.
/// Для 500 текст исключения наружу не отдаётся — он остаётся только в логе.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>Создаёт middleware.</summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Оборачивает следующий компонент pipeline в глобальную обработку исключений.</summary>
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
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Некорректный запрос"),
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
            Detail = statusCode == StatusCodes.Status500InternalServerError ? null : exception.Message,
            Instance = context.Request.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
    }
}
