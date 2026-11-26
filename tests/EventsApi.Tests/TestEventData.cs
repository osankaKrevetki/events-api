using EventsApi.Dtos;
using EventsApi.Models;
using EventsApi.Services;

namespace EventsApi.Tests;

/// <summary>
/// Заготовки мероприятий с фиксированными датами и фабрики сервисов с предзаполненными данными.
/// </summary>
internal static class TestEventData
{
    public static readonly DateTime Jan10 = new(2026, 1, 10, 10, 0, 0);
    public static readonly DateTime Jan12 = new(2026, 1, 12, 18, 0, 0);
    public static readonly DateTime Feb1 = new(2026, 2, 1, 10, 0, 0);
    public static readonly DateTime Feb3 = new(2026, 2, 3, 18, 0, 0);
    public static readonly DateTime Mar5 = new(2026, 3, 5, 12, 0, 0);
    public static readonly DateTime Mar6 = new(2026, 3, 6, 15, 0, 0);

    /// <summary>Мероприятие по умолчанию: валидное, с нейтральными датами.</summary>
    public static Event MakeEvent(
        string title = "Мероприятие",
        DateTime? start = null,
        DateTime? end = null) => new()
    {
        Title = title,
        Description = "Описание",
        StartAt = start ?? Jan10,
        EndAt = end ?? Jan12
    };

    /// <summary>DotNet Conf: 10–12 января.</summary>
    public static Event DotNetConf() => new()
    {
        Title = "DotNet Conf",
        Description = "Конференция по .NET",
        StartAt = Jan10,
        EndAt = Jan12
    };

    /// <summary>Python Meetup: 1–3 февраля.</summary>
    public static Event PythonMeetup() => new()
    {
        Title = "Python Meetup",
        StartAt = Feb1,
        EndAt = Feb3
    };

    /// <summary>dotnet party: 5–6 марта.</summary>
    public static Event DotNetParty() => new()
    {
        Title = "dotnet party",
        StartAt = Mar5,
        EndAt = Mar6
    };

    /// <summary>Сервис с тремя мероприятиями: DotNet Conf, Python Meetup, dotnet party.</summary>
    public static EventService ServiceWithThreeEvents()
    {
        var service = new EventService();
        service.Create(DotNetConf());
        service.Create(PythonMeetup());
        service.Create(DotNetParty());
        return service;
    }

    /// <summary>Сервис с <paramref name="count"/> мероприятиями, начинающимися строго по одному в день с 1 января 2026.</summary>
    public static EventService ServiceWithEvents(int count)
    {
        var service = new EventService();
        var start = new DateTime(2026, 1, 1, 10, 0, 0);

        for (var i = 0; i < count; i++)
        {
            service.Create(MakeEvent($"Мероприятие {i + 1:00}", start.AddDays(i), start.AddDays(i).AddHours(2)));
        }

        return service;
    }

    /// <summary>Запрос без фильтров с указанной страницей и размером страницы.</summary>
    public static EventQuery Page(int page = 1, int pageSize = 10) => new()
    {
        Page = page,
        PageSize = pageSize
    };
}
