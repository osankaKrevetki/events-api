namespace EventsApi.Models;

/// <summary>
/// Мероприятие. Неизменяемая модель: изменения оформляются созданием новой записи (выражение with),
/// поэтому наружу можно безопасно отдавать ссылки на хранимые объекты.
/// </summary>
public record Event
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; init; }

    /// <summary>Название.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Описание.</summary>
    public string? Description { get; init; }

    /// <summary>Дата и время начала.</summary>
    public DateTime StartAt { get; init; }

    /// <summary>Дата и время окончания.</summary>
    public DateTime EndAt { get; init; }
}
