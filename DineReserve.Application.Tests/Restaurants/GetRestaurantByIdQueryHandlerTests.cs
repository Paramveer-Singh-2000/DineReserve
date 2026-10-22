using DineReserve.Application.Restaurants.Queries.GetRestaurantById;
using DineReserve.Application.Tests.TestHelpers;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

public class GetRestaurantByIdQueryHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly GetRestaurantByIdQueryHandler _sut;

    public GetRestaurantByIdQueryHandlerTests()
    {
        _sut = new GetRestaurantByIdQueryHandler(
            Substitute.For<ILogger<GetRestaurantByIdQueryHandler>>(),
            MapperTestHelper.CreateMapper(),
            _restaurantRepositories);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRestaurantDoesNotExist()
    {
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns((Restaurant?)null);

        var act = () => _sut.Handle(new GetRestaurantByIdQuery { Id = 1 }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsMappedDto_IncludingAddressAndDishes()
    {
        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Tasty Place",
            Description = "Great food",
            Category = "Italian",
            Address = new Address { Street = "1 Main St", City = "LA", State = "CA", Country = "USA", ZipCode = "90001" },
            Dishes = [new Dish { Id = 1, Name = "Pizza", Price = 12m }]
        };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);

        var result = await _sut.Handle(new GetRestaurantByIdQuery { Id = 1 }, CancellationToken.None);

        result.Name.Should().Be("Tasty Place");
        result.Street.Should().Be("1 Main St");
        result.Dishes.Should().ContainSingle(d => d.Name == "Pizza");
    }

    [Fact]
    public async Task Handle_MapsNullAddressFields_AsNull_NotThrow()
    {
        var restaurant = new Restaurant { Id = 1, Name = "X", Description = "Y", Category = "Italian", Address = null };
        _restaurantRepositories.GetRestaurantByIdAsync(1).Returns(restaurant);

        var result = await _sut.Handle(new GetRestaurantByIdQuery { Id = 1 }, CancellationToken.None);

        result.Street.Should().BeNull();
        result.City.Should().BeNull();
    }
}
