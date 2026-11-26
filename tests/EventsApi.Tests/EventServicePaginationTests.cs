using EventsApi.Dtos;
using EventsApi.Services;
using FluentAssertions;
using Xunit;

namespace EventsApi.Tests;

/// <summary>
/// Пагинация мероприятий в GetEvents.
/// </summary>
public class EventServicePaginationTests
{
    [Fact]
    public void DefaultPagination_ReturnsFirstTenEventsOrderedByStart()
    {
        // Arrange
        var service = TestEventData.ServiceWithEvents(25);

        // Act
        var result = service.GetEvents(new EventQuery());

        // Assert
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Items.Should().HaveCount(10);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.Items.First().Title.Should().Be("Мероприятие 01");
    }

    [Fact]
    public void SecondPage_ReturnsNextTenEvents()
    {
        // Arrange
        var service = TestEventData.ServiceWithEvents(25);

        // Act
        var result = service.GetEvents(TestEventData.Page(page: 2));

        // Assert
        result.Page.Should().Be(2);
        result.Items.Should().HaveCount(10);
        result.Items.First().Title.Should().Be("Мероприятие 11");
    }

    [Fact]
    public void LastPage_ReturnsRemainingEvents()
    {
        // Arrange
        var service = TestEventData.ServiceWithEvents(25);

        // Act
        var result = service.GetEvents(TestEventData.Page(page: 3));

        // Assert
        result.Items.Should().HaveCount(5);
        result.Items.First().Title.Should().Be("Мероприятие 21");
        result.Items.Last().Title.Should().Be("Мероприятие 25");
    }

    [Fact]
    public void PageBeyondLast_ReturnsEmptyItemsButKeepsTotalCount()
    {
        // Arrange
        var service = TestEventData.ServiceWithEvents(25);

        // Act
        var result = service.GetEvents(TestEventData.Page(page: 99));

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(25);
    }

    [Fact]
    public void PageSize_LimitsItemsOnPage()
    {
        // Arrange
        var service = TestEventData.ServiceWithEvents(25);

        // Act
        var result = service.GetEvents(TestEventData.Page(pageSize: 7));

        // Assert
        result.Items.Should().HaveCount(7);
        result.TotalPages.Should().Be(4);
    }

    [Fact]
    public void Items_AreOrderedByStartDateAscending()
    {
        // Arrange
        var service = TestEventData.ServiceWithEvents(25);

        // Act
        var result = service.GetEvents(TestEventData.Page(pageSize: 25));

        // Assert
        result.Items.Should().BeInAscendingOrder(e => e.StartAt);
    }
}
