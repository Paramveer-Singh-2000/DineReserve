using System.Security.Claims;
using DineReserve.Application.Users;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using DineReserve.Infrastructure.Authorization.Requirements;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Authorization;

public class CreateMultipleRestaurantRequirementHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly CreateMultipleRestaurantRequirementHandler _sut;

    public CreateMultipleRestaurantRequirementHandlerTests()
    {
        _sut = new CreateMultipleRestaurantRequirementHandler(_restaurantRepositories, _userContext);
    }

    private static AuthorizationHandlerContext CreateContext(CreateMultipleRestaurantRequirement requirement) =>
        new([requirement], new ClaimsPrincipal(new ClaimsIdentity()), resource: null);

    private static Restaurant OwnedBy(string ownerId) => new() { Name = "R", Description = "d", Category = "c", OwnerId = ownerId };

    [Fact]
    public async Task Succeeds_WhenUserOwnsAtLeastTheRequiredNumberOfRestaurants()
    {
        _userContext.GetCurrentUser().Returns(new CurrentUser("owner-1", "a@test.com", [], null, null));
        _restaurantRepositories.GetAllAsync().Returns([OwnedBy("owner-1"), OwnedBy("owner-1"), OwnedBy("someone-else")]);
        var requirement = new CreateMultipleRestaurantRequirement(2);
        var context = CreateContext(requirement);

        await _sut.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task Fails_WhenUserOwnsFewerThanRequired()
    {
        _userContext.GetCurrentUser().Returns(new CurrentUser("owner-1", "a@test.com", [], null, null));
        _restaurantRepositories.GetAllAsync().Returns([OwnedBy("owner-1"), OwnedBy("someone-else")]);
        var requirement = new CreateMultipleRestaurantRequirement(2);
        var context = CreateContext(requirement);

        await _sut.HandleAsync(context);

        context.HasFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Fails_WhenUserOwnsNoRestaurants()
    {
        _userContext.GetCurrentUser().Returns(new CurrentUser("owner-1", "a@test.com", [], null, null));
        _restaurantRepositories.GetAllAsync().Returns([]);
        var context = CreateContext(new CreateMultipleRestaurantRequirement(1));

        await _sut.HandleAsync(context);

        context.HasFailed.Should().BeTrue();
    }
}
