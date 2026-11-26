using EventsApi.Dtos;
using EventsApi.Services;
using FluentAssertions;
using Xunit;

namespace EventsApi.Tests;

/// <summary>
/// Фильтрация мероприятий в GetEvents: по названию, датам и их комбинациям.
/// </summary>
public class EventServiceFilteringTests
{
    [Fact]
    public void TitleFilter_IsCaseInsensitiveAndPartialMatch()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery { Title = "dotnet" };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.TotalCount.Should().Be(2);
        result.Items.Select(e => e.Title).Should().Contain("DotNet Conf").And.Contain("dotnet party");
    }

    [Fact]
    public void TitleFilter_WithoutMatch_ReturnsEmptyResult()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery { Title = "java" };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void TitleFilter_EmptyOrWhitespace_IgnoredAndReturnsAll(string title)
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery { Title = title };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public void FromFilter_ReturnsEventsStartingNotEarlier()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery { From = TestEventData.Feb1 };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.Items.Select(e => e.Title).Should().Contain("Python Meetup").And.Contain("dotnet party");
        result.Items.Should().NotContain(e => e.Title == "DotNet Conf");
    }

    [Fact]
    public void FromFilter_BoundaryDate_IncludesEventStartingExactlyAt()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery { From = TestEventData.Feb1 };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.Items.Should().Contain(e => e.Title == "Python Meetup" && e.StartAt == TestEventData.Feb1);
    }

    [Fact]
    public void ToFilter_ReturnsEventsEndingNotLater()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery { To = TestEventData.Feb3 };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.Items.Select(e => e.Title).Should().Contain("DotNet Conf").And.Contain("Python Meetup");
        result.Items.Should().NotContain(e => e.Title == "dotnet party");
    }

    [Fact]
    public void ToFilter_BoundaryDate_IncludesEventEndingExactlyAt()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery { To = TestEventData.Feb3 };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.Items.Should().Contain(e => e.Title == "Python Meetup" && e.EndAt == TestEventData.Feb3);
    }

    [Fact]
    public void FromAndToFilters_ReturnEventsWithinRange()
    {
        // Arrange
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery { From = TestEventData.Jan12, To = TestEventData.Mar5 };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.Items.Single().Title.Should().Be("Python Meetup");
    }

    [Fact]
    public void CombinedFilters_WorkTogetherAsLogicalAnd()
    {
        // Arrange
        // title "dotnet" отсекает Python Meetup, From = 1 февраля отсекает DotNet Conf,
        // To = 6 марта по граничной дате включает dotnet party.
        var service = TestEventData.ServiceWithThreeEvents();
        var query = new EventQuery
        {
            Title = "dotnet",
            From = TestEventData.Feb1,
            To = TestEventData.Mar6
        };

        // Act
        var result = service.GetEvents(query);

        // Assert
        result.Items.Single().Title.Should().Be("dotnet party");
    }
}
