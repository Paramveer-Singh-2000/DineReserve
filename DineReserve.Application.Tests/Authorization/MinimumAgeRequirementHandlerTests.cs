using System.Security.Claims;
using DineReserve.Application.Users;
using DineReserve.Infrastructure.Authorization.Requirements;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Authorization;

public class MinimumAgeRequirementHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly MinimumAgeRequirementHandler _sut;

    public MinimumAgeRequirementHandlerTests()
    {
        _sut = new MinimumAgeRequirementHandler(Substitute.For<ILogger<MinimumAgeRequirementHandler>>(), _userContext);
    }

    private static AuthorizationHandlerContext CreateContext(MinimumAgeRequirement requirement) =>
        new([requirement], new ClaimsPrincipal(new ClaimsIdentity()), resource: null);

    [Fact]
    public async Task Succeeds_WhenUserIsOldEnough()
    {
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        _userContext.GetCurrentUser().Returns(new CurrentUser("u1", "a@test.com", [], null, birthDate));
        var requirement = new MinimumAgeRequirement(20);
        var context = CreateContext(requirement);

        await _sut.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task Fails_WhenUserIsTooYoung()
    {
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-15));
        _userContext.GetCurrentUser().Returns(new CurrentUser("u1", "a@test.com", [], null, birthDate));
        var requirement = new MinimumAgeRequirement(20);
        var context = CreateContext(requirement);

        await _sut.HandleAsync(context);

        context.HasFailed.Should().BeTrue();
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task NeitherSucceedsNorFails_WhenDateOfBirthIsNotSet()
    {
        _userContext.GetCurrentUser().Returns(new CurrentUser("u1", "a@test.com", [], null, null));
        var requirement = new MinimumAgeRequirement(20);
        var context = CreateContext(requirement);

        await _sut.HandleAsync(context);

        // No DOB claim means the handler can neither confirm nor deny - it leaves the
        // requirement pending rather than failing outright, letting another handler
        // (or policy combination) decide.
        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeFalse();
    }

    [Fact]
    public async Task Succeeds_WhenUserTurns20ExactlyToday()
    {
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
        _userContext.GetCurrentUser().Returns(new CurrentUser("u1", "a@test.com", [], null, birthDate));
        var requirement = new MinimumAgeRequirement(20);
        var context = CreateContext(requirement);

        await _sut.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue("the requirement uses <=, so turning 20 exactly today should satisfy 'at least 20'");
    }
}
