namespace EventsApi.Exceptions;

/// <summary>
/// Исключение выбрасывается, когда запрошенный ресурс не найден.
/// Глобальный обработчик ошибок преобразует его в ответ 404 Not Found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
