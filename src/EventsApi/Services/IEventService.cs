using EventsApi.Dtos;
using EventsApi.Exceptions;
using EventsApi.Models;

namespace EventsApi.Services;

/// <summary>
/// Сервис для работы с мероприятиями
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Возвращает страницу мероприятий с учётом фильтрации и пагинации.
    /// Фильтры применяются только если переданы и объединяются по логическому И.
    /// </summary>
    /// <exception cref="ValidationException">Некорректные параметры пагинации.</exception>
    PaginatedResult<Event> GetEvents(EventQuery query);

    /// <summary>Возвращает мероприятие по идентификатору.</summary>
    /// <exception cref="NotFoundException">Мероприятие не найдено.</exception>
    Event GetById(Guid id);

    /// <summary>Создаёт мероприятие.</summary>
    /// <exception cref="ValidationException">Некорректные данные мероприятия.</exception>
    Event Create(Event newEvent);

    /// <summary>
    /// Полностью обновляет мероприятие
    /// </summary>
    /// <returns>Обновленное мероприятие</returns>
    /// <exception cref="NotFoundException">Мероприятие не найдено.</exception>
    /// <exception cref="ValidationException">Некорректные данные мероприятия.</exception>
    Event Update(Guid id, Event updatedEvent);

    /// <summary>Удаляет мероприятие.</summary>
    /// <exception cref="NotFoundException">Мероприятие не найдено.</exception>
    void Delete(Guid id);
}
