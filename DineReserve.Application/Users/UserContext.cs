
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace DineReserve.Application.Users
{
    public interface IUserContext
    {
        public CurrentUser? GetCurrentUser();
    }

    public class UserContext(IHttpContextAccessor? _httpContextAccessor) : IUserContext
    {
        public CurrentUser? GetCurrentUser()
        {
            var user = _httpContextAccessor?.HttpContext?.User;

            if (user is null)
            {
                throw new InvalidOperationException("No HttpContext or User available.");
            }

            if (user?.Identities is null || !user.Identity?.IsAuthenticated == true)
            {
                return null;
            }
            var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
            var roles = user.FindAll(c => c.Type == ClaimTypes.Role)
                 .Select(r => r.Value)
                 .ToList();
            var nationality = user.FindFirst(c => c.Type == "Nationality")?.Value;
            var dateOfBirthString = user.FindFirst(c => c.Type == "DateOfBirth")?.Value;
            var dateOfBirth = dateOfBirthString == null ? (DateOnly?)null : DateOnly.ParseExact(dateOfBirthString, "yyyy-MM-dd");
            return new CurrentUser(userId!, email, roles, nationality, dateOfBirth);
        }
    }
}
