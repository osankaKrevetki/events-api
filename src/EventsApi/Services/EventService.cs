using EventsApi.Dtos;
using EventsApi.Exceptions;
using EventsApi.Models;

namespace EventsApi.Services;

/// <summary>
/// Реализация IEventService с хранением данных в памяти приложения
/// </summary>
public class EventService : IEventService
{
    private readonly List<Event> _events = new();
    private readonly object _lock = new();

    public PaginatedResult<Event> GetEvents(EventQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.Page < 1)
        {
            throw new ValidationException("Параметр page должен быть не меньше 1.");
        }

        if (query.PageSize < 1 || query.PageSize > EventQuery.MaxPageSize)
        {
            throw new ValidationException($"Параметр pageSize должен быть от 1 до {EventQuery.MaxPageSize}.");
        }

        lock (_lock)
        {
            IEnumerable<Event> filtered = _events;

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                var title = query.Title.Trim();
                filtered = filtered.Where(e => e.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            if (query.From.HasValue)
            {
                filtered = filtered.Where(e => e.StartAt >= query.From.Value);
            }

            if (query.To.HasValue)
            {
                filtered = filtered.Where(e => e.EndAt <= query.To.Value);
            }

            var totalCount = filtered.Count();

            var items = filtered
                .OrderBy(e => e.StartAt)
                .ThenBy(e => e.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new PaginatedResult<Event>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }

    public Event GetById(Guid id)
    {
        lock (_lock)
        {
            return _events.FirstOrDefault(e => e.Id == id)
                ?? throw new NotFoundException($"Мероприятие с id '{id}' не найдено.");
        }
    }

    public Event Create(Event newEvent)
    {
        Validate(newEvent);

        newEvent.Id = Guid.NewGuid();

        lock (_lock)
        {
            _events.Add(newEvent);
        }

        return newEvent;
    }

    public Event Update(Guid id, Event updatedEvent)
    {
        Validate(updatedEvent);

        lock (_lock)
        {
            var existing = _events.FirstOrDefault(e => e.Id == id)
                ?? throw new NotFoundException($"Мероприятие с id '{id}' не найдено.");

            existing.Title = updatedEvent.Title;
            existing.Description = updatedEvent.Description;
            existing.StartAt = updatedEvent.StartAt;
            existing.EndAt = updatedEvent.EndAt;

            return existing;
        }
    }

    public void Delete(Guid id)
    {
        lock (_lock)
        {
            if (_events.RemoveAll(e => e.Id == id) == 0)
            {
                throw new NotFoundException($"Мероприятие с id '{id}' не найдено.");
            }
        }
    }

    /// <summary>
    /// Проверяет бизнес-правила: название непустое и не длиннее 200 символов,
    /// дата окончания позже даты начала.
    /// </summary>
    private static void Validate(Event model)
    {
        var title = model.Title?.Trim() ?? string.Empty;

        if (title.Length == 0)
        {
            throw new ValidationException("Название мероприятия (Title) обязательно.");
        }

        if (title.Length > EventRequest.TitleMaxLength)
        {
            throw new ValidationException(
                $"Название мероприятия (Title) не должно быть длиннее {EventRequest.TitleMaxLength} символов.");
        }

        if (model.EndAt <= model.StartAt)
        {
            throw new ValidationException("Дата окончания (EndAt) должна быть позже даты начала (StartAt).");
        }
    }
}
