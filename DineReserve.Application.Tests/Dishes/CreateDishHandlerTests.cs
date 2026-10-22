using DineReserve.Application.Dishes.Commands.CreateDish;
using DineReserve.Application.Tests.TestHelpers;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Dishes;

public class CreateDishHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly IDishesRepositories _dishesRepositories = Substitute.For<IDishesRepositories>();
    private readonly CreateDishHandler _sut;

    public CreateDishHandlerTests()
    {
        _sut = new CreateDishHandler(
            Substitute.For<ILogger<CreateDishHandler>>(),
            MapperTestHelper.CreateMapper(),
            _restaurantRepositories,
            _dishesRepositories);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRestaurantDoesNotExist()
    {
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns((Restaurant?)null);

        var act = () => _sut.Handle(new CreateDishCommand { RestaurantId = 1, Name = "Fries", Price = 4.99m }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await _dishesRepositories.DidNotReceive().Create(Arg.Any<Dish>());
    }

    [Fact]
    public async Task Handle_CreatesDish_MappedFromCommand()
    {
        var restaurant = new Restaurant { Id = 1, Name = "X", Description = "Y", Category = "Italian" };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);
        _dishesRepositories.Create(Arg.Any<Dish>()).Returns(callInfo => Task.FromResult(7));

        var result = await _sut.Handle(new CreateDishCommand { RestaurantId = 1, Name = "Fries", Description = "Crispy", Price = 4.99m, KiloCalories = 300 }, CancellationToken.None);

        result.Should().Be(7);
        await _dishesRepositories.Received(1).Create(Arg.Is<Dish>(d =>
            d.Name == "Fries" && d.Description == "Crispy" && d.Price == 4.99m && d.KiloCalories == 300));
    }
}
