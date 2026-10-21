using EventsApi.Models;

namespace EventsApi.Services;

/// <summary>
/// Сервис для работы с мероприятиями
/// </summary>
public interface IEventService
{
    IReadOnlyCollection<Event> GetAll();
    Event? GetById(Guid id);
    Event Create(Event newEvent);

    /// <summary>
    /// Полностью обновляет мероприятие
    /// </summary>
    /// <returns>Обновленное мероприятие или <c>null</c>, есил мероприятие не найдено</returns>
    Event? Update(Guid id, Event updatedEvent);

    /// <returns><c>true</c>, если мероприятие было удалено; <c>false</c>, если не найдено</returns>
    bool Delete(Guid id);
}
