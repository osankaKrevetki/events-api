namespace EventsApi.Models;

/// <summary>
/// Единые правила валидации мероприятия. Используются одновременно DTO EventRequest
/// (ранняя проверка на уровне API) и сервисом EventService, чтобы лимиты и тексты ошибок не расходились.
/// </summary>
public static class EventValidation
{
    /// <summary>Максимальная длина названия мероприятия.</summary>
    public const int TitleMaxLength = 200;

    /// <summary>Сообщение о пустом названии.</summary>
    public const string TitleRequiredMessage = "Название мероприятия (Title) обязательно.";

    /// <summary>Сообщение о слишком длинном названии.</summary>
    public const string TitleTooLongMessage = "Название мероприятия (Title) не должно быть длиннее 200 символов.";

    /// <summary>Сообщение о дате окончания не позже даты начала.</summary>
    public const string EndBeforeStartMessage = "Дата окончания (EndAt) должна быть позже даты начала (StartAt).";

    /// <summary>
    /// Проверяет название: не пустое после обрезки пробелов и не длиннее <see cref="TitleMaxLength"/>.
    /// </summary>
    /// <returns>Текст ошибки или <c>null</c>, если название корректно.</returns>
    public static string? GetTitleError(string? title)
    {
        var trimmed = title?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
        {
            return TitleRequiredMessage;
        }

        if (trimmed.Length > TitleMaxLength)
        {
            return TitleTooLongMessage;
        }

        return null;
    }

    /// <summary>Дата окончания должна быть строго позже даты начала.</summary>
    public static bool IsEndAfterStart(DateTime startAt, DateTime endAt) => endAt > startAt;
}
