namespace EventsApi.Exceptions;

/// <summary>
/// Исключение выбрасывается при некорректных входных данных.
/// Глобальный обработчик ошибок преобразует его в ответ 400 Bad Request.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>Создаёт исключение с текстом ошибки валидации.</summary>
    public ValidationException(string message) : base(message)
    {
    }
}
