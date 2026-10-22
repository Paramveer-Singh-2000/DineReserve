using DineReserve.Application.Restaurants.Commands.CreateRestaurant;
using DineReserve.Application.Tests.TestHelpers;
using DineReserve.Application.Users;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

public class CreateRestaurantHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly CreateRestaurantHandler _sut;

    public CreateRestaurantHandlerTests()
    {
        _sut = new CreateRestaurantHandler(
            _userContext,
            Substitute.For<ILogger<CreateRestaurantHandler>>(),
            MapperTestHelper.CreateMapper(),
            _restaurantRepositories);
    }

    [Fact]
    public async Task Handle_SetsOwnerId_ToCurrentUser()
    {
        _userContext.GetCurrentUser().Returns(new CurrentUser("owner-42", "owner@test.com", [], null, null));
        _restaurantRepositories.CreateRestaurantAsync(Arg.Any<Restaurant>()).Returns(callInfo => Task.FromResult(((Restaurant)callInfo[0]).Id));

        var command = new CreateRestaurantCommand
        {
            Name = "New Place",
            Description = "Great food",
            Category = "Italian"
        };

        await _sut.Handle(command, CancellationToken.None);

        await _restaurantRepositories.Received(1).CreateRestaurantAsync(
            Arg.Is<Restaurant>(r => r.OwnerId == "owner-42" && r.Name == "New Place"));
    }

    [Fact]
    public async Task Handle_ReturnsIdFromRepository()
    {
        _userContext.GetCurrentUser().Returns(new CurrentUser("owner-1", "owner@test.com", [], null, null));
        _restaurantRepositories.CreateRestaurantAsync(Arg.Any<Restaurant>()).Returns(123);

        var result = await _sut.Handle(new CreateRestaurantCommand { Name = "X", Description = "Y", Category = "Mexican" }, CancellationToken.None);

        result.Should().Be(123);
    }

    [Fact]
    public async Task Handle_MapsAddressFields_IntoNestedAddress()
    {
        _userContext.GetCurrentUser().Returns(new CurrentUser("owner-1", "owner@test.com", [], null, null));
        Restaurant? captured = null;
        _restaurantRepositories.CreateRestaurantAsync(Arg.Do<Restaurant>(r => captured = r)).Returns(1);

        var command = new CreateRestaurantCommand
        {
            Name = "X",
            Description = "Y",
            Category = "Mexican",
            Street = "123 Main St",
            City = "Thousand Oaks",
            State = "CA",
            Country = "USA",
            ZipCode = "91320"
        };

        await _sut.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Address.Should().NotBeNull();
        captured.Address!.Street.Should().Be("123 Main St");
        captured.Address.City.Should().Be("Thousand Oaks");
    }
}
