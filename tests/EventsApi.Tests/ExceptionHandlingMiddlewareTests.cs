using System.Text.Json;
using EventsApi.Exceptions;
using EventsApi.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EventsApi.Tests;

/// <summary>
/// Тесты глобального обработчика исключений: статус-коды, content-type и состав тела Problem Details.
/// </summary>
public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task ValidationException_Returns400ProblemDetails()
    {
        // Arrange
        var exception = new ValidationException("Параметр page должен быть не меньше 1.");

        // Act
        var (statusCode, contentType, body) = await InvokeAsync(exception, "/events");

        // Assert
        statusCode.Should().Be(StatusCodes.Status400BadRequest);
        contentType.Should().Be("application/problem+json");
        body.RootElement.GetProperty("title").GetString().Should().Be("Ошибка валидации");
        body.RootElement.GetProperty("detail").GetString().Should().Be(exception.Message);
        body.RootElement.GetProperty("instance").GetString().Should().Be("/events");
        body.RootElement.GetProperty("traceId").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task NotFoundException_Returns404ProblemDetails()
    {
        // Arrange
        var exception = new NotFoundException("Мероприятие с id '00000000-0000-0000-0000-000000000000' не найдено.");

        // Act
        var (statusCode, contentType, body) = await InvokeAsync(exception, "/events/00000000-0000-0000-0000-000000000000");

        // Assert
        statusCode.Should().Be(StatusCodes.Status404NotFound);
        contentType.Should().Be("application/problem+json");
        body.RootElement.GetProperty("title").GetString().Should().Be("Ресурс не найден");
        body.RootElement.GetProperty("detail").GetString().Should().Be(exception.Message);
        body.RootElement.GetProperty("traceId").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task BadHttpRequestException_Returns400()
    {
        // Arrange
        var exception = new BadHttpRequestException("Unexpected end of request content");

        // Act
        var (statusCode, _, _) = await InvokeAsync(exception);

        // Assert
        statusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UnexpectedException_Returns500WithoutExceptionDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("Секретная внутренняя ошибка");

        // Act
        var (statusCode, contentType, body) = await InvokeAsync(exception, "/events");

        // Assert
        statusCode.Should().Be(StatusCodes.Status500InternalServerError);
        contentType.Should().Be("application/problem+json");
        body.RootElement.GetProperty("title").GetString().Should().Be("Внутренняя ошибка сервера");
        body.RootElement.TryGetProperty("detail", out _).Should().BeFalse("текст исключения не должен уходить наружу");
        body.RootElement.ToString().Should().NotContain("Секретная");
        body.RootElement.GetProperty("traceId").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task NoException_PassesRequestToNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var middleware = new ExceptionHandlingMiddleware(
            async ctx => await Task.Run(() => ctx.Response.StatusCode = StatusCodes.Status200OK),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        context.Response.Body.Length.Should().Be(0);
    }

    /// <summary>Прогоняет middleware с делегатом, выбрасывающим <paramref name="exception"/>, и разбирает ответ.</summary>
    private static async Task<(int StatusCode, string? ContentType, JsonDocument Body)> InvokeAsync(
        Exception exception, string path = "/events")
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => Task.FromException(exception),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        var body = await JsonDocument.ParseAsync(context.Response.Body);
        return (context.Response.StatusCode, context.Response.ContentType, body);
    }
}
