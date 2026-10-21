namespace EventsApi.Dtos;

/// <summary>
/// Мероприятие в ответе API
/// </summary>
public record EventResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime StartAt,
    DateTime EndAt);
