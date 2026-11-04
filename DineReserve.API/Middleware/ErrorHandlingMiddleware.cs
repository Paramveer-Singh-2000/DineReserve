
using DineReserve.Domain.Exceptions;
using FluentValidation;

namespace DineReserve.API.Middleware;

public class ErrorHandlingMiddleware(
    ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status404NotFound,
                ex.Message);
        }
        catch (ForbidException ex)
        {
            logger.LogWarning(
                "Forbidden request at {Path}: {Error}",
                context.Request.Path,
                ex.Message);

            await WriteErrorAsync(
                context,
                StatusCodes.Status403Forbidden,
                "You are not allowed to perform this operation.");
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(
                "Validation failed for {Path}",
                context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                message = "Validation failed.",
                errors = ex.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    error = e.ErrorMessage
                })
            });
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception at {Path}",
                context.Request.Path);

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected server error occurred.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        if (context.Response.HasStarted)
            throw new InvalidOperationException(
                "Cannot write error response after response started.");

        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            message
        });
    }
}
