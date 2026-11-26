using EventsApi.Dtos;
using EventsApi.Exceptions;
using EventsApi.Services;
using FluentAssertions;
using Xunit;

namespace EventsApi.Tests;

/// <summary>
/// Неуспешные сценарии: отсутствующие мероприятия, некорректные данные и параметры пагинации.
/// </summary>
public class EventServiceValidationTests
{
    [Fact]
    public void GetById_UnknownId_ThrowsNotFoundException()
    {
        // Arrange
        var service = new EventService();

        // Act
        var act = () => service.GetById(Guid.NewGuid());

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage("*не найдено*");
    }

    [Fact]
    public void Update_UnknownId_ThrowsNotFoundException()
    {
        // Arrange
        var service = new EventService();

        // Act
        var act = () => service.Update(Guid.NewGuid(), TestEventData.MakeEvent());

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage("*не найдено*");
    }

    [Fact]
    public void Delete_UnknownId_ThrowsNotFoundException()
    {
        // Arrange
        var service = new EventService();

        // Act
        var act = () => service.Delete(Guid.NewGuid());

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage("*не найдено*");
    }

    [Fact]
    public void Create_EndAtBeforeStartAt_ThrowsValidationException()
    {
        // Arrange
        var service = new EventService();
        var newEvent = TestEventData.MakeEvent(
            start: TestEventData.Feb1,
            end: TestEventData.Jan10);

        // Act
        var act = () => service.Create(newEvent);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*дата окончания*");
    }

    [Fact]
    public void Create_EndAtEqualToStartAt_ThrowsValidationException()
    {
        // Arrange
        var service = new EventService();
        var newEvent = TestEventData.MakeEvent(
            start: TestEventData.Feb1,
            end: TestEventData.Feb1);

        // Act
        var act = () => service.Create(newEvent);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*дата окончания*");
    }

    [Fact]
    public void Update_EndAtBeforeStartAt_ThrowsValidationException()
    {
        // Arrange
        var service = new EventService();
        var created = service.Create(TestEventData.DotNetConf());
        var updated = TestEventData.MakeEvent(
            start: TestEventData.Mar5,
            end: TestEventData.Feb1);

        // Act
        var act = () => service.Update(created.Id, updated);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*дата окончания*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyOrWhitespaceTitle_ThrowsValidationException(string title)
    {
        // Arrange
        var service = new EventService();
        var newEvent = TestEventData.MakeEvent(title);

        // Act
        var act = () => service.Create(newEvent);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*название*");
    }

    [Fact]
    public void Create_TitleLongerThanMax_ThrowsValidationException()
    {
        // Arrange
        var service = new EventService();
        var newEvent = TestEventData.MakeEvent(new string('а', 201));

        // Act
        var act = () => service.Create(newEvent);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*200*");
    }

    [Fact]
    public void GetEvents_PageLessThanOne_ThrowsValidationException()
    {
        // Arrange
        var service = new EventService();
        var query = TestEventData.Page(page: 0);

        // Act
        var act = () => service.GetEvents(query);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*page*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void GetEvents_PageSizeOutOfRange_ThrowsValidationException(int pageSize)
    {
        // Arrange
        var service = new EventService();
        var query = TestEventData.Page(pageSize: pageSize);

        // Act
        var act = () => service.GetEvents(query);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*pageSize*");
    }

    [Fact]
    public void GetEvents_NullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new EventService();

        // Act
        var act = () => service.GetEvents(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
