using Microsoft.AspNetCore.Mvc;

namespace EventsApi.Results;

/// <summary>
/// Записывает Problem Details напрямую в ответ с content-type application/problem+json,
/// минуя форматтеры вывода MVC: они отдают только application/json.
/// Используется для ошибок валидации и биндинга, чтобы формат совпадал с ответами middleware.
/// </summary>
public class ProblemDetailsResult : IActionResult
{
    private readonly int _statusCode;
    private readonly object _problemDetails;

    /// <summary>Создаёт результат с телом Problem Details и кодом ответа.</summary>
    public ProblemDetailsResult(int statusCode, object problemDetails)
    {
        _statusCode = statusCode;
        _problemDetails = problemDetails;
    }

    /// <inheritdoc />
    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.StatusCode = _statusCode;
        await response.WriteAsJsonAsync(_problemDetails, options: null, contentType: "application/problem+json");
    }
}
