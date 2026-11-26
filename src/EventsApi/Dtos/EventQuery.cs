namespace EventsApi.Dtos;

/// <summary>
/// Параметры выборки мероприятий: фильтрация и пагинация.
/// </summary>
public class EventQuery
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    /// <summary>Поиск по названию (регистронезависимый, частичное совпадение).</summary>
    public string? Title { get; init; }

    /// <summary>События, которые начинаются не раньше указанной даты.</summary>
    public DateTime? From { get; init; }

    /// <summary>События, которые заканчиваются не позже указанной даты.</summary>
    public DateTime? To { get; init; }

    /// <summary>Номер страницы, начиная с 1.</summary>
    public int Page { get; init; } = DefaultPage;

    /// <summary>Количество элементов на странице, от 1 до <see cref="MaxPageSize"/>.</summary>
    public int PageSize { get; init; } = DefaultPageSize;
}
