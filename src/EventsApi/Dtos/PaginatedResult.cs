namespace EventsApi.Dtos;

/// <summary>
/// Страница результатов пагинации: сами элементы и служебные поля.
/// </summary>
public record PaginatedResult<T>
{
    /// <summary>Элементы текущей страницы; их количество — число элементов на странице.</summary>
    public required IReadOnlyCollection<T> Items { get; init; }

    /// <summary>Общее количество элементов, удовлетворяющих фильтрам (по всем страницам).</summary>
    public int TotalCount { get; init; }

    /// <summary>Номер текущей страницы, начиная с 1.</summary>
    public int Page { get; init; }

    /// <summary>Количество элементов на странице.</summary>
    public int PageSize { get; init; }

    /// <summary>Общее количество страниц (0, если выборка пуста).</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}
