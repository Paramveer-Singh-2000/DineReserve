using System.Text.Json;
using DineReserve.API.Middleware;
using DineReserve.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DineReserve.Application.Tests.Middleware;

public class ErrorHandlingMiddlewareTests
{
    private readonly ErrorHandlingMiddleware _sut = new();

    private static async Task<(int StatusCode, string Body)> InvokeAndCapture(ErrorHandlingMiddleware middleware, RequestDelegate next)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context, next);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();
        return (context.Response.StatusCode, body);
    }

    [Fact]
    public async Task NotFoundException_ResultsIn404_WithMessageInBody()
    {
        var (statusCode, body) = await InvokeAndCapture(_sut, _ => throw new NotFoundException("Restaurant", "123"));

        statusCode.Should().Be(404);
        using var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("message").GetString().Should().Contain("Restaurant").And.Contain("123");
    }

    [Fact]
    public async Task ForbidException_ResultsIn403_WithItsOwnMessage_NotAGenericOne()
    {
        var (statusCode, body) = await InvokeAndCapture(_sut, _ => throw new ForbidException("You are not allowed to delete restaurant with id: 5."));

        statusCode.Should().Be(403);
        using var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("message").GetString().Should().Be("You are not allowed to delete restaurant with id: 5.");
    }

    [Fact]
    public async Task UnexpectedException_ResultsIn500()
    {
        var (statusCode, _) = await InvokeAndCapture(_sut, _ => throw new InvalidOperationException("boom"));

        statusCode.Should().Be(500);
    }

    [Fact]
    public async Task NoException_PassesThroughUntouched()
    {
        var context = new DefaultHttpContext();
        var nextCalled = false;

        await _sut.InvokeAsync(context, _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200); // untouched default
    }
}
