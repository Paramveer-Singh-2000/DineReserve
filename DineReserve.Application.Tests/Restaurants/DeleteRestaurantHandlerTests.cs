using DineReserve.Application.Restaurants.Commands.DeleteRestaurant;
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

public class DeleteRestaurantHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly IRestaurantAuthorizationService _authorizationService = Substitute.For<IRestaurantAuthorizationService>();
    private readonly DeleteRestaurantHandler _sut;

    public DeleteRestaurantHandlerTests()
    {
        _sut = new DeleteRestaurantHandler(
            Substitute.For<ILogger<DeleteRestaurantHandler>>(),
            _restaurantRepositories,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRestaurantDoesNotExist()
    {
        _restaurantRepositories.GetRestaurantByIdAsync(99).Returns((Restaurant?)null);

        var act = () => _sut.Handle(new DeleteRestaurantCommand(99), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ThrowsForbidden_WhenNotAuthorized()
    {
        var restaurant = new Restaurant { Id = 1, Name = "X", Description = "Y", Category = "Italian" };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);
        _authorizationService.Authorize(restaurant, ResourceOperations.Delete).Returns(false);

        var act = () => _sut.Handle(new DeleteRestaurantCommand(1), CancellationToken.None);

        await act.Should().ThrowAsync<ForbidException>();
        await _restaurantRepositories.DidNotReceive().DeleteRestaurantAsync(Arg.Any<Restaurant>());
    }

    [Fact]
    public async Task Handle_DeletesRestaurant_WhenAuthorized()
    {
        var restaurant = new Restaurant { Id = 1, Name = "X", Description = "Y", Category = "Italian" };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);
        _authorizationService.Authorize(restaurant, ResourceOperations.Delete).Returns(true);

        await _sut.Handle(new DeleteRestaurantCommand(1), CancellationToken.None);

        await _restaurantRepositories.Received(1).DeleteRestaurantAsync(restaurant);
    }
}
