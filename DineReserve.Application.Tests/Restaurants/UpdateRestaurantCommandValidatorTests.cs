using DineReserve.Application.Restaurants.Commands.UpdateRestaurant;
using FluentAssertions;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

public class UpdateRestaurantCommandValidatorTests
{
    private readonly UpdateRestaurantCommandValidator _sut = new();

    [Theory]
    [InlineData("")]
    [InlineData("ab")] // below the 3-character minimum
    public void Name_Invalid_ReturnsError(string name)
    {
        var result = _sut.Validate(new UpdateRestaurantCommand { Name = name, Description = "A valid description" });

        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRestaurantCommand.Name));
    }

    [Fact]
    public void Name_TooLong_ReturnsError()
    {
        var result = _sut.Validate(new UpdateRestaurantCommand { Name = new string('a', 101), Description = "valid" });

        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRestaurantCommand.Name));
    }

    [Fact]
    public void Description_Empty_ReturnsError()
    {
        var result = _sut.Validate(new UpdateRestaurantCommand { Name = "Valid Name", Description = "" });

        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRestaurantCommand.Description));
    }

    [Fact]
    public void ValidCommand_HasNoErrors()
    {
        var result = _sut.Validate(new UpdateRestaurantCommand { Name = "Valid Name", Description = "A valid description" });

        result.IsValid.Should().BeTrue();
    }
}
