
using DineReserve.Domain.Exceptions;

namespace DineReserve.API.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(NotFoundException nfEx)
            {
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                var response = new { Message = nfEx.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
            catch(ForbidException fEx)
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var response = new { Message = "An unexpected error occurred.", Details = fEx.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var response = new { Message = "An unexpected error occurred.", Details = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
