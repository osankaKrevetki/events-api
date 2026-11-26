using EventsApi.Dtos;
using EventsApi.Exceptions;
using EventsApi.Models;

namespace EventsApi.Services;

/// <summary>
/// Реализация IEventService с хранением данных в памяти приложения.
/// Модель Event неизменяемая, поэтому наружу допустимо отдавать ссылки на хранимые объекты.
/// </summary>
public class EventService : IEventService
{
    private readonly List<Event> _events = new();
    private readonly object _lock = new();

    /// <inheritdoc />
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

    /// <inheritdoc />
    public Event GetById(Guid id)
    {
        lock (_lock)
        {
            return _events.FirstOrDefault(e => e.Id == id)
                ?? throw new NotFoundException($"Мероприятие с id '{id}' не найдено.");
        }
    }

    /// <inheritdoc />
    public Event Create(Event newEvent)
    {
        Validate(newEvent);

        var created = newEvent with { Id = Guid.NewGuid() };

        lock (_lock)
        {
            _events.Add(created);
        }

        return created;
    }

    /// <inheritdoc />
    public Event Update(Guid id, Event updatedEvent)
    {
        lock (_lock)
        {
            var index = _events.FindIndex(e => e.Id == id);
            if (index < 0)
            {
                throw new NotFoundException($"Мероприятие с id '{id}' не найдено.");
            }

            Validate(updatedEvent);

            var updated = updatedEvent with { Id = id };
            _events[index] = updated;

            return updated;
        }
    }

    /// <inheritdoc />
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
    /// Проверяет бизнес-правила мероприятия — см. <see cref="EventValidation"/>.
    /// </summary>
    private static void Validate(Event model)
    {
        var titleError = EventValidation.GetTitleError(model.Title);
        if (titleError is not null)
        {
            throw new ValidationException(titleError);
        }

        if (!EventValidation.IsEndAfterStart(model.StartAt, model.EndAt))
        {
            throw new ValidationException(EventValidation.EndBeforeStartMessage);
        }
    }
}
