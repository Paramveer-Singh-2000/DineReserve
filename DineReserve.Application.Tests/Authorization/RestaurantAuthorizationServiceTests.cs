using DineReserve.Application.Users;
using DineReserve.Domain.Authorization;
using DineReserve.Domain.Constants;
using DineReserve.Domain.Entities;
using DineReserve.Infrastructure.Authorization.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Authorization;

/// <summary>
/// Regression tests for a real bug found in the original code: due to C# operator
/// precedence, "Delete || Update && isOwner" was parsed as "Delete || (Update && isOwner)",
/// which let ANY authenticated user delete ANY restaurant regardless of ownership.
/// These tests pin down the corrected behavior: (Delete || Update) && isOwner.
/// </summary>
public class RestaurantAuthorizationServiceTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly RestaurantAuthorizationService _sut;

    private static readonly Restaurant OwnedRestaurant = new()
    {
        Id = 1,
        Name = "Test Restaurant",
        Description = "desc",
        Category = "Italian",
        OwnerId = "owner-1"
    };

    public RestaurantAuthorizationServiceTests()
    {
        _sut = new RestaurantAuthorizationService(Substitute.For<ILogger<RestaurantAuthorizationService>>(), _userContext);
    }

    private void ActingAs(string userId, params string[] roles)
    {
        _userContext.GetCurrentUser().Returns(new CurrentUser(userId, "user@test.com", roles, null, null));
    }

    [Fact]
    public void NonOwner_CannotDelete_Restaurant()
    {
        ActingAs("some-other-user");

        var result = _sut.Authorize(OwnedRestaurant, ResourceOperations.Delete);

        result.Should().BeFalse("a non-owner, non-admin user must not be able to delete someone else's restaurant");
    }

    [Fact]
    public void NonOwner_CannotUpdate_Restaurant()
    {
        ActingAs("some-other-user");

        var result = _sut.Authorize(OwnedRestaurant, ResourceOperations.Update);

        result.Should().BeFalse();
    }

    [Fact]
    public void Owner_CanDelete_OwnRestaurant()
    {
        ActingAs("owner-1");

        var result = _sut.Authorize(OwnedRestaurant, ResourceOperations.Delete);

        result.Should().BeTrue();
    }

    [Fact]
    public void Owner_CanUpdate_OwnRestaurant()
    {
        ActingAs("owner-1");

        var result = _sut.Authorize(OwnedRestaurant, ResourceOperations.Update);

        result.Should().BeTrue();
    }

    [Fact]
    public void Admin_CanDelete_AnyRestaurant()
    {
        ActingAs("admin-1", UserRoles.Admin);

        var result = _sut.Authorize(OwnedRestaurant, ResourceOperations.Delete);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(ResourceOperations.Create)]
    [InlineData(ResourceOperations.Read)]
    public void AnyUser_CanCreateOrRead(ResourceOperations operation)
    {
        ActingAs("anyone");

        var result = _sut.Authorize(OwnedRestaurant, operation);

        result.Should().BeTrue();
    }
}
