using AutoMapper;
using DineReserve.Application.Restaurants;
using DineReserve.Application.Restaurants.Commands.UpdateRestaurant;
using DineReserve.Domain.Authorization;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using DineReserve.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

/// <summary>
/// Regression test for a real bug found in the original code: UpdateRestaurantHandler
/// authorized against ResourceOperations.Delete instead of ResourceOperations.Update.
/// </summary>
public class UpdateRestaurantHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly IRestaurantAuthorizationService _authorizationService = Substitute.For<IRestaurantAuthorizationService>();
    private readonly IMapper _mapper;
    private readonly UpdateRestaurantHandler _sut;

    public UpdateRestaurantHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<RestaurantProfile>());
        _mapper = config.CreateMapper();
        _sut = new UpdateRestaurantHandler(
            Substitute.For<ILogger<UpdateRestaurantHandler>>(),
            _mapper,
            _restaurantRepositories,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_ChecksUpdatePermission_NotDeletePermission()
    {
        var restaurant = new Restaurant { Id = 1, Name = "Old Name", Description = "desc", Category = "Italian" };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);
        _authorizationService.Authorize(restaurant, ResourceOperations.Update).Returns(true);

        await _sut.Handle(new UpdateRestaurantCommand { Id = 1, Name = "New Name", Description = "new desc" }, CancellationToken.None);

        _authorizationService.Received(1).Authorize(restaurant, ResourceOperations.Update);
        _authorizationService.DidNotReceive().Authorize(restaurant, ResourceOperations.Delete);
        restaurant.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task Handle_ThrowsForbidden_WhenNotAuthorizedForUpdate()
    {
        var restaurant = new Restaurant { Id = 1, Name = "Old Name", Description = "desc", Category = "Italian" };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);
        _authorizationService.Authorize(restaurant, ResourceOperations.Update).Returns(false);

        var act = () => _sut.Handle(new UpdateRestaurantCommand { Id = 1, Name = "New Name", Description = "new desc" }, CancellationToken.None);

        await act.Should().ThrowAsync<ForbidException>();
    }
}
