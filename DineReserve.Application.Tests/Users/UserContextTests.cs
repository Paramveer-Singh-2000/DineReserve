using System.Security.Claims;
using DineReserve.Application.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Users;

public class UserContextTests
{
    private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
    private readonly UserContext _sut;

    public UserContextTests()
    {
        _sut = new UserContext(_httpContextAccessor);
    }

    private static ClaimsPrincipal AuthenticatedPrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void GetCurrentUser_ReturnsNull_WhenUserIsNotAuthenticated()
    {
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) };
        _httpContextAccessor.HttpContext.Returns(httpContext);

        var result = _sut.GetCurrentUser();

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentUser_Throws_WhenNoHttpContextIsAvailable()
    {
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        var act = () => _sut.GetCurrentUser();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetCurrentUser_MapsAllClaims_ForAuthenticatedUser()
    {
        var principal = AuthenticatedPrincipal(
            new Claim(ClaimTypes.NameIdentifier, "user-42"),
            new Claim(ClaimTypes.Email, "user@test.com"),
            new Claim(ClaimTypes.Role, "Owner"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("Nationality", "USA"),
            new Claim("DateOfBirth", "1995-06-15"));
        _httpContextAccessor.HttpContext.Returns(new DefaultHttpContext { User = principal });

        var result = _sut.GetCurrentUser();

        result.Should().NotBeNull();
        result!.UserId.Should().Be("user-42");
        result.Email.Should().Be("user@test.com");
        result.Roles.Should().BeEquivalentTo(["Owner", "User"]);
        result.Nationality.Should().Be("USA");
        result.DateOfBirth.Should().Be(new DateOnly(1995, 6, 15));
        result.IsInRole("Owner").Should().BeTrue();
        result.IsInRole("Admin").Should().BeFalse();
    }

    [Fact]
    public void GetCurrentUser_LeavesOptionalClaimsNull_WhenNotPresent()
    {
        var principal = AuthenticatedPrincipal(new Claim(ClaimTypes.NameIdentifier, "user-1"));
        _httpContextAccessor.HttpContext.Returns(new DefaultHttpContext { User = principal });

        var result = _sut.GetCurrentUser();

        result!.Nationality.Should().BeNull();
        result.DateOfBirth.Should().BeNull();
        result.Email.Should().BeEmpty();
    }
}
