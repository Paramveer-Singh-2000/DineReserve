
using DineReserve.Application.Users;
using DineReserve.Domain.Constants;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Xunit;

namespace DineReserve.Application.Tests;

public class UserContextTests
{
    [Fact]
    public void GetCurrentUser_AuthenticatedUser_ReturnsExpectedUser()
    {
        // Arrange
        var dob = new DateOnly(1990, 1, 1);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "123"),
            new(ClaimTypes.Name, "John Doe"),
            new(ClaimTypes.Email, "john.doe@example.com"),
            new(ClaimTypes.Role, UserRoles.Admin),
            new(ClaimTypes.Role, UserRoles.User),
            new("Nationality", "American"),
            new("DateOfBirth", dob.ToString("yyyy-MM-dd"))
        };

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, "TestAuth"));

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var userContext = new UserContext(accessor);

        // Act
        var currentUser = userContext.GetCurrentUser();

        // Assert
        currentUser.Should().NotBeNull();
        currentUser!.UserId.Should().Be("123");
        currentUser.Email.Should().Be("john.doe@example.com");
        currentUser.Roles.Should()
            .ContainInOrder(UserRoles.Admin, UserRoles.User);
        currentUser.Nationality.Should().Be("American");
        currentUser.DateOfBirth.Should().Be(dob);
    }
}
