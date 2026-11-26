using EventsApi.Models;

namespace EventsApi.Dtos;

/// <summary>
/// Маппинг между DTO и доменной моделью
/// </summary>
public static class EventMappings
{
    /// <summary>Собирает доменную модель из запроса на создание или обновление.</summary>
    public static Event ToModel(this EventRequest request) => new()
    {
        Title = request.Title!.Trim(),
        Description = request.Description,
        StartAt = request.StartAt!.Value,
        EndAt = request.EndAt!.Value
    };

    /// <summary>Собирает ответ API из доменной модели.</summary>
    public static EventResponse ToResponse(this Event model) => new()
    {
        Id = model.Id,
        Title = model.Title,
        Description = model.Description,
        StartAt = model.StartAt,
        EndAt = model.EndAt
    };

    /// <summary>
    /// Переводит страницу доменных моделей в страницу DTO, сохраняя метаданные пагинации.
    /// </summary>
    public static PaginatedResult<TResult> Map<TSource, TResult>(
        this PaginatedResult<TSource> source, Func<TSource, TResult> mapper) => new()
        {
            Items = source.Items.Select(mapper).ToList(),
            TotalCount = source.TotalCount,
            Page = source.Page,
            PageSize = source.PageSize
        };
}
