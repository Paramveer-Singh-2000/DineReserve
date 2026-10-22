using DineReserve.Application.Dishes.Queries.GetDishById;
using DineReserve.Application.Tests.TestHelpers;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Dishes;

public class GetDishByIdQueryHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly GetDishByIdQueryHandler _sut;

    public GetDishByIdQueryHandlerTests()
    {
        _sut = new GetDishByIdQueryHandler(
            Substitute.For<ILogger<GetDishByIdQueryHandler>>(),
            MapperTestHelper.CreateMapper(),
            _restaurantRepositories);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRestaurantDoesNotExist()
    {
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns((Restaurant?)null);

        var act = () => _sut.Handle(new GetDishByIdQuery(1, 5), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenDishDoesNotBelongToRestaurant()
    {
        var restaurant = new Restaurant { Id = 1, Name = "X", Description = "Y", Category = "Italian", Dishes = [] };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);

        var act = () => _sut.Handle(new GetDishByIdQuery(1, 999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsDish_WhenFound()
    {
        var dish = new Dish { Id = 5, Name = "Fries", Price = 3.5m };
        var restaurant = new Restaurant { Id = 1, Name = "X", Description = "Y", Category = "Italian", Dishes = [dish] };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);

        var result = await _sut.Handle(new GetDishByIdQuery(1, 5), CancellationToken.None);

        result.Name.Should().Be("Fries");
        result.Price.Should().Be(3.5m);
    }
}
