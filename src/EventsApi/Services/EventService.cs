using EventsApi.Models;

namespace EventsApi.Services;

/// <summary>
/// Реализация IEventService с хранением данных в памяти приложения
/// </summary>
public class EventService : IEventService
{
    private readonly List<Event> _events = new();
    private readonly object _lock = new();

    public IReadOnlyCollection<Event> GetAll()
    {
        lock (_lock)
        {
            return _events.ToList();
        }
    }

    public Event? GetById(Guid Id)
    {
        lock (_lock)
        {
            return _events.FirstOrDefault(e => e.Id == Id);
        }
    }

    public Event Create(Event newEvent)
    {
        newEvent.Id = Guid.NewGuid();

        lock (_lock)
        {
            _events.Add(newEvent);
        }

        return newEvent;
    }

    public Event? Update(Guid id, Event updatedEvent)
    {
        lock (_lock)
        {
            var existing = _events.FirstOrDefault(e => e.Id == id);
            if (existing is null)
            {
                return null;
            }

            existing.Title = updatedEvent.Title;
            existing.Description = updatedEvent.Description;
            existing.StartAt = updatedEvent.StartAt;
            existing.EndAt = updatedEvent.EndAt;

            return existing;
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            return _events.RemoveAll(e => e.Id == id) > 0;
        }
    }
}
