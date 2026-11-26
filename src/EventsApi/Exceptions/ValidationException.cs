namespace EventsApi.Exceptions;

/// <summary>
/// Исключение выбрасывается при некорректных входных данных.
/// Глобальный обработчик ошибок преобразует его в ответ 400 Bad Request.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
