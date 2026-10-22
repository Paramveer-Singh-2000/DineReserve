using DineReserve.Application.Dishes.Commands.DeleteDish;
using DineReserve.Domain.Authorization;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using DineReserve.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Dishes;

/// <summary>
/// Regression tests for a real bug found in the original code: DeleteDishCommand's handler
/// looked up and deleted a *restaurant* using the dish's id (treating the dish id as if it
/// were a restaurant id), instead of deleting the dish itself. These tests confirm the fix:
/// deleting a dish only removes the dish, never the restaurant.
/// </summary>
public class DeleteDishCommandHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly IDishesRepositories _dishesRepositories = Substitute.For<IDishesRepositories>();
    private readonly IRestaurantAuthorizationService _authorizationService = Substitute.For<IRestaurantAuthorizationService>();
    private readonly DeleteDishCommandHandler _sut;

    public DeleteDishCommandHandlerTests()
    {
        _sut = new DeleteDishCommandHandler(
            Substitute.For<ILogger<DeleteDishCommandHandler>>(),
            _restaurantRepositories,
            _dishesRepositories,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_DeletesOnlyTheDish_NeverTheRestaurant()
    {
        var dish = new Dish { Id = 42, Name = "Fries", RestaurantId = 1 };
        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Test Restaurant",
            Description = "desc",
            Category = "American",
            Dishes = [dish]
        };

        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);
        _authorizationService.Authorize(restaurant, ResourceOperations.Delete).Returns(true);

        await _sut.Handle(new DeleteDishCommand(restaurantId: 1, dishId: 42), CancellationToken.None);

        await _dishesRepositories.Received(1).DeleteDishAsync(Arg.Is<IEnumerable<Dish>>(d => d.Single().Id == 42));
        await _restaurantRepositories.DidNotReceive().DeleteRestaurantAsync(Arg.Any<Restaurant>());
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenDishDoesNotBelongToRestaurant()
    {
        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Test Restaurant",
            Description = "desc",
            Category = "American",
            Dishes = []
        };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);

        var act = () => _sut.Handle(new DeleteDishCommand(restaurantId: 1, dishId: 999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await _dishesRepositories.DidNotReceive().DeleteDishAsync(Arg.Any<IEnumerable<Dish>>());
    }

    [Fact]
    public async Task Handle_ThrowsForbidden_WhenUserDoesNotOwnRestaurant()
    {
        var dish = new Dish { Id = 42, Name = "Fries", RestaurantId = 1 };
        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Test Restaurant",
            Description = "desc",
            Category = "American",
            Dishes = [dish]
        };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);
        _authorizationService.Authorize(restaurant, ResourceOperations.Delete).Returns(false);

        var act = () => _sut.Handle(new DeleteDishCommand(restaurantId: 1, dishId: 42), CancellationToken.None);

        await act.Should().ThrowAsync<ForbidException>();
        await _dishesRepositories.DidNotReceive().DeleteDishAsync(Arg.Any<IEnumerable<Dish>>());
    }
}
