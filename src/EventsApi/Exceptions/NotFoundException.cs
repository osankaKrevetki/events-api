namespace EventsApi.Exceptions;

/// <summary>
/// Исключение выбрасывается, когда запрошенный ресурс не найден.
/// Глобальный обработчик ошибок преобразует его в ответ 404 Not Found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>Создаёт исключение с сообщением о ненайденном ресурсе.</summary>
    public NotFoundException(string message) : base(message)
    {
    }
}
