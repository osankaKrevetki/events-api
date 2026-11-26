using System.ComponentModel.DataAnnotations;
using EventsApi.Models;

namespace EventsApi.Dtos;

/// <summary>
/// Данные для создания или полного обновления мероприятия.
/// Правила валидации общие с сервисом — см. <see cref="EventValidation"/>.
/// </summary>
public class EventRequest : IValidatableObject
{
    /// <summary>Название мероприятия.</summary>
    /// <example>Конференция .NET</example>
    [Required(ErrorMessage = EventValidation.TitleRequiredMessage)]
    public string? Title { get; set; }

    /// <summary>Описание мероприятия (необязательно).</summary>
    /// <example>Доклады о новинках ASP.NET Core</example>
    public string? Description { get; set; }

    /// <summary>Дата и время начала.</summary>
    /// <example>2026-11-01T10:00:00</example>
    [Required(ErrorMessage = "Поле StartAt обязательно.")]
    public DateTime? StartAt { get; set; }

    /// <summary>Дата и время окончания. Должно быть позже StartAt.</summary>
    /// <example>2026-11-01T18:00:00</example>
    [Required(ErrorMessage = "Поле EndAt обязательно.")]
    public DateTime? EndAt { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var titleError = EventValidation.GetTitleError(Title);
        if (titleError is not null)
        {
            yield return new ValidationResult(titleError, new[] { nameof(Title) });
        }

        if (StartAt.HasValue && EndAt.HasValue && !EventValidation.IsEndAfterStart(StartAt.Value, EndAt.Value))
        {
            yield return new ValidationResult(EventValidation.EndBeforeStartMessage, new[] { nameof(EndAt) });
        }
    }
}
