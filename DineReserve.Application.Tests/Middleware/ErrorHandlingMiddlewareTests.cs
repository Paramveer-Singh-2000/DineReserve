using System.Text.Json;
using DineReserve.API.Middleware;
using DineReserve.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;

namespace DineReserve.Application.Tests.Middleware;

public class ErrorHandlingMiddlewareTests
{
    private readonly ErrorHandlingMiddleware _sut =
    new(NullLogger<ErrorHandlingMiddleware>.Instance);

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
    public async Task ForbidException_Returns403_WithoutExposingDetails()
    {
        // Arrange
        var privateMessage =
            "You are not allowed to delete restaurant with id: 5.";

        // Act
        var (statusCode, body) = await InvokeAndCapture(
            _sut,
            _ => throw new ForbidException(privateMessage));

        // Assert
        statusCode.Should().Be(403);

        using var json = JsonDocument.Parse(body);

        var message = json.RootElement
            .GetProperty("message")
            .GetString();

        message.Should().Be(
    "You are not allowed to perform this operation.");

        body.Should().NotContain(privateMessage);
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

    [Fact]
    public async Task UnexpectedException_Returns500_WithoutExposingDetails()
    {
        // Arrange
        const string sensitiveError =
            "Database password=SuperSecret123";

        // Act
        var (statusCode, body) = await InvokeAndCapture(
            _sut,
            _ => throw new InvalidOperationException(sensitiveError));

        // Assert
        statusCode.Should().Be(500);

        using var json = JsonDocument.Parse(body);

        var message = json.RootElement
            .GetProperty("message")
            .GetString();

        message.Should().Be(
            "An unexpected server error occurred.");

        body.Should().NotContain(sensitiveError);
    }

    
using System.Text.Json;
using DineReserve.API.Middleware;
using DineReserve.Domain.Exceptions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DineReserve.Application.Tests.Middleware;

public class ErrorHandlingMiddlewareTests
{
    private readonly ErrorHandlingMiddleware _sut =
        new(NullLogger<ErrorHandlingMiddleware>.Instance);

    private static async Task<(int StatusCode, string Body)> InvokeAndCapture(
        ErrorHandlingMiddleware middleware,
        RequestDelegate next)
    {
        var context = new DefaultHttpContext();

        using var stream = new MemoryStream();
        context.Response.Body = stream;

        await middleware.InvokeAsync(context, next);

        stream.Position = 0;

        using var reader = new StreamReader(stream);
        var body = await reader.ReadToEndAsync();

        return (context.Response.StatusCode, body);
    }

    [Fact]
    public async Task NotFoundException_Returns404()
    {
        // Arrange & Act
        var (statusCode, body) = await InvokeAndCapture(
            _sut,
            _ => throw new NotFoundException("Restaurant", "123"));

        // Assert
        statusCode.Should().Be(404);

        using var json = JsonDocument.Parse(body);

        json.RootElement
            .GetProperty("message")
            .GetString()
            .Should().Contain("Restaurant");
    }

    [Fact]
    public async Task ForbidException_Returns403_WithoutExposingDetails()
    {
        // Arrange
        const string internalMessage =
            "User cannot delete restaurant 5.";

        // Act
        var (statusCode, body) = await InvokeAndCapture(
            _sut,
            _ => throw new ForbidException(internalMessage));

        // Assert
        statusCode.Should().Be(403);

        using var json = JsonDocument.Parse(body);

        json.RootElement
            .GetProperty("message")
            .GetString()
            .Should().Be(
                "You are not allowed to perform this operation.");

        body.Should().NotContain(internalMessage);
    }

    [Fact]
    public async Task ValidationException_Returns400_WithErrors()
    {
        // Arrange
        var failures = new[]
        {
            new ValidationFailure(
                "Name", "Name is required.")
        };

        // Act
        var (statusCode, body) = await InvokeAndCapture(
            _sut,
            _ => throw new ValidationException(failures));

        // Assert
        statusCode.Should().Be(400);

        using var json = JsonDocument.Parse(body);

        json.RootElement
            .GetProperty("message")
            .GetString()
            .Should().Be("Validation failed.");

        var errors = json.RootElement
            .GetProperty("errors");

        errors.GetArrayLength().Should().Be(1);

        errors[0]
            .GetProperty("field")
            .GetString()
            .Should().Be("Name");

        errors[0]
            .GetProperty("error")
            .GetString()
            .Should().Be("Name is required.");
    }

    [Fact]
    public async Task UnexpectedException_Returns500_WithoutSensitiveData()
    {
        // Arrange
        const string sensitiveMessage =
            "Database connection failed. Password=secret123";

        // Act
        var (statusCode, body) = await InvokeAndCapture(
            _sut,
            _ => throw new InvalidOperationException(sensitiveMessage));

        // Assert
        statusCode.Should().Be(500);

        using var json = JsonDocument.Parse(body);

        json.RootElement
            .GetProperty("message")
            .GetString()
            .Should().Be(
                "An unexpected server error occurred.");

        body.Should().NotContain(sensitiveMessage);
        body.Should().NotContain("Password");
    }

    [Fact]
    public async Task NoException_ContinuesNormally()
    {
        // Arrange
        var nextCalled = false;

        // Act
        var (statusCode, _) = await InvokeAndCapture(
            _sut,
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        // Assert
        nextCalled.Should().BeTrue();
        statusCode.Should().Be(200);
    }
}


}
