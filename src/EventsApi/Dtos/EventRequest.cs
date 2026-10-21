using System.ComponentModel.DataAnnotations;

namespace EventsApi.Dtos;

/// <summary>
/// Данные для создания или полного обновления мероприятия.
/// </summary>
public class EventRequest : IValidatableObject
{
    public const int TitleMaxLength = 200;

    /// <summary>Название мероприятия.</summary>
    /// <example>Конференция .NET</example>
    [Required(ErrorMessage = "Поле Title обязательно.")]
    [StringLength(TitleMaxLength, ErrorMessage = "Поле Title не должно быть длиннее {1} символов.")]
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartAt.HasValue && EndAt.HasValue && EndAt.Value <= StartAt.Value)
        {
            yield return new ValidationResult(
                "Дата окончания (EndAt) должна быть позже даты начала (StartAt).",
                new[] { nameof(EndAt) });
        }
    }
}
