using EventsApi.Models;

namespace EventsApi.Dtos;

/// <summary>
/// Маппинг между DTO и доменной моделью
/// </summary>
public static class EventMappings
{
    public static Event ToModel(this EventRequest request) => new()
    {
        Title = request.Title!.Trim(),
        Description = request.Description,
        StartAt = request.StartAt!.Value,
        EndAt = request.EndAt!.Value
    };

    public static EventResponse ToResponse(this Event model) =>
        new(model.Id, model.Title, model.Description, model.StartAt, model.EndAt);
}
