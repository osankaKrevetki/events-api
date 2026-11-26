using EventsApi.Dtos;
using EventsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventsApi.Controllers;

/// <summary>
/// CRUD-операции над мероприятиями.
/// </summary>
[ApiController]
[Route("events")]
[Produces("application/json")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Получить постраничный список мероприятий с фильтрацией.
    /// Фильтры применяются только если переданы и объединяются по логическому И.
    /// </summary>
    /// <param name="title">Поиск по названию (регистронезависимый, частичное совпадение).</param>
    /// <param name="from">События, которые начинаются не раньше указанной даты.</param>
    /// <param name="to">События, которые заканчиваются не позже указанной даты.</param>
    /// <param name="page">Номер страницы, начиная с 1.</param>
    /// <param name="pageSize">Количество элементов на странице, от 1 до 100.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<EventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<PaginatedResult<EventResponse>> GetAll(
        [FromQuery] string? title = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = EventQuery.DefaultPage,
        [FromQuery] int pageSize = EventQuery.DefaultPageSize)
    {
        var query = new EventQuery
        {
            Title = title,
            From = from,
            To = to,
            Page = page,
            PageSize = pageSize
        };

        var result = _eventService.GetEvents(query);
        return Ok(result.Map(e => e.ToResponse()));
    }

    /// <summary>Получить мероприятие по идентификатору.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<EventResponse> GetById(Guid id)
    {
        return Ok(_eventService.GetById(id).ToResponse());
    }

    /// <summary>Создать мероприятие.</summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<EventResponse> Create([FromBody] EventRequest request)
    {
        var created = _eventService.Create(request.ToModel());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse());
    }

    /// <summary>Полностью обновить мероприятие.</summary>
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<EventResponse> Update(Guid id, [FromBody] EventRequest request)
    {
        var updated = _eventService.Update(id, request.ToModel());
        return Ok(updated.ToResponse());
    }

    /// <summary>Удалить мероприятие.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        _eventService.Delete(id);
        return NoContent();
    }
}
