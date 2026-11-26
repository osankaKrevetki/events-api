namespace EventsApi.Dtos;

/// <summary>
/// Мероприятие в ответе API
/// </summary>
public record EventResponse
{
    /// <summary>Идентификатор мероприятия.</summary>
    public required Guid Id { get; init; }

    /// <summary>Название мероприятия.</summary>
    public required string Title { get; init; }

    /// <summary>Описание мероприятия.</summary>
    public string? Description { get; init; }

    /// <summary>Дата и время начала.</summary>
    public required DateTime StartAt { get; init; }

    /// <summary>Дата и время окончания.</summary>
    public required DateTime EndAt { get; init; }
}
