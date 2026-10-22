using DineReserve.Application.Dishes.Queries.GetAllDishes;
using DineReserve.Application.Tests.TestHelpers;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Dishes;

public class GetAllDishesQueryHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly GetAllDishesQueryHandler _sut;

    public GetAllDishesQueryHandlerTests()
    {
        _sut = new GetAllDishesQueryHandler(
            Substitute.For<ILogger<GetAllDishesQueryHandler>>(),
            MapperTestHelper.CreateMapper(),
            _restaurantRepositories);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRestaurantDoesNotExist()
    {
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns((Restaurant?)null);

        var act = () => _sut.Handle(new GetAllDishesQuery(1), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsAllDishes_ForRestaurant()
    {
        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "X",
            Description = "Y",
            Category = "Italian",
            Dishes =
            [
                new Dish { Id = 1, Name = "Pizza", Price = 12m },
                new Dish { Id = 2, Name = "Pasta", Price = 10m }
            ]
        };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);

        var result = await _sut.Handle(new GetAllDishesQuery(1), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(d => d.Name).Should().BeEquivalentTo(["Pizza", "Pasta"]);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenRestaurantHasNoDishes()
    {
        var restaurant = new Restaurant { Id = 1, Name = "X", Description = "Y", Category = "Italian", Dishes = [] };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);

        var result = await _sut.Handle(new GetAllDishesQuery(1), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
