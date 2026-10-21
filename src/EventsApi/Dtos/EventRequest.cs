namespace EventsApi.Dtos;

/// <summary>
/// Данные для создания или полного обновления мероприятия
/// </summary>
public class EventRequest
{
    /// <summary>
    /// Название мероприятия
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Описание мероприятия
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Дата и время начала
    /// </summary>
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Дата и время окончания. Должно быть позже <see cref="StartAt"/>
    /// </summary>
    public DateTime? EndAt { get; set; }
}
