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

    /// <summary>Получить список всех мероприятий.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<EventResponse>> GetAll()
    {
        var events = _eventService.GetAll().Select(e => e.ToResponse());
        return Ok(events);
    }

    /// <summary>Получить мероприятие по идентификатору.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<EventResponse> GetById(Guid id)
    {
        var found = _eventService.GetById(id);
        return found is null ? EventNotFound(id) : Ok(found.ToResponse());
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
        return updated is null ? EventNotFound(id) : Ok(updated.ToResponse());
    }

    /// <summary>Удалить мероприятие.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        return _eventService.Delete(id) ? NoContent() : EventNotFound(id);
    }

    private ObjectResult EventNotFound(Guid id) =>
        Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Мероприятие не найдено",
            detail: $"Мероприятие с id '{id}' не найдено.");
}
