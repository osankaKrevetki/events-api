using EventsApi.Dtos;
using EventsApi.Exceptions;
using EventsApi.Services;
using FluentAssertions;
using Xunit;

namespace EventsApi.Tests;

/// <summary>
/// Успешные сценарии CRUD-операций EventService.
/// </summary>
public class EventServiceCrudTests
{
    [Fact]
    public void Create_AssignsIdAndStoresEvent()
    {
        // Arrange
        var service = new EventService();
        var newEvent = TestEventData.MakeEvent();

        // Act
        var created = service.Create(newEvent);

        // Assert
        created.Id.Should().NotBeEmpty();
        service.GetById(created.Id).Should().BeEquivalentTo(created);
    }

    [Fact]
    public void Create_ReturnsEventWithGivenFields()
    {
        // Arrange
        var service = new EventService();
        var newEvent = TestEventData.DotNetConf();

        // Act
        var created = service.Create(newEvent);

        // Assert
        created.Title.Should().Be("DotNet Conf");
        created.Description.Should().Be("Конференция по .NET");
        created.StartAt.Should().Be(TestEventData.Jan10);
        created.EndAt.Should().Be(TestEventData.Jan12);
    }

    [Fact]
    public void Create_TwoEvents_GetDifferentIds()
    {
        // Arrange
        var service = new EventService();

        // Act
        var first = service.Create(TestEventData.DotNetConf());
        var second = service.Create(TestEventData.PythonMeetup());

        // Assert
        first.Id.Should().NotBe(second.Id);
    }

    [Fact]
    public void GetEvents_WithoutFilters_ReturnsAllCreatedEvents()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();

        // Act
        var result = service.GetEvents(new EventQuery());

        // Assert
        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public void GetById_ReturnsCreatedEvent()
    {
        // Arrange
        var service = new EventService();
        var created = service.Create(TestEventData.PythonMeetup());

        // Act
        var found = service.GetById(created.Id);

        // Assert
        found.Should().BeEquivalentTo(created);
    }

    [Fact]
    public void Update_ReplacesAllEditableFields()
    {
        // Arrange
        var service = new EventService();
        var created = service.Create(TestEventData.DotNetConf());
        var updated = TestEventData.MakeEvent(
            "DotNet Conf 2027",
            TestEventData.Feb1,
            TestEventData.Feb3);

        // Act
        var result = service.Update(created.Id, updated);

        // Assert
        result.Id.Should().Be(created.Id);
        result.Title.Should().Be("DotNet Conf 2027");
        result.StartAt.Should().Be(TestEventData.Feb1);
        result.EndAt.Should().Be(TestEventData.Feb3);
        service.GetById(created.Id).Should().BeEquivalentTo(result);
    }

    [Fact]
    public void Delete_RemovesEvent()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var created = service.Create(TestEventData.MakeEvent("Лишнее мероприятие"));

        // Act
        service.Delete(created.Id);

        // Assert
        service.GetEvents(new EventQuery()).TotalCount.Should().Be(3);
        var act = () => service.GetById(created.Id);
        act.Should().Throw<NotFoundException>();
    }
}
