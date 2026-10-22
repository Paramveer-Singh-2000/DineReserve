using DineReserve.Application.Dishes.Commands.CreateDish;
using FluentAssertions;
using Xunit;

namespace DineReserve.Application.Tests.Dishes;

public class CreateDishCommandValidatorTests
{
    public readonly CreateDishCommandValidator _sut = new();

    private static CreateDishCommand ValidCommand() => new()
    {
        Name = "Fries",
        Description = "Crispy fries",
        Price = 4.99m,
        RestaurantId = 1
    };

    [Fact]
    public void Name_Empty_ReturnsError()
    {
        var command = ValidCommand();
        command.Name = "";

        var result = _sut.Validate(command);

        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDishCommand.Name));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Price_NotPositive_ReturnsError(decimal price)
    {
        var command = ValidCommand();
        command.Price = price;

        var result = _sut.Validate(command);

        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDishCommand.Price));
    }

    [Fact]
    public void KiloCalories_Null_IsValid()
    {
        var command = ValidCommand();
        command.KiloCalories = null;

        var result = _sut.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateDishCommand.KiloCalories));
    }

    [Fact]
    public void KiloCalories_ZeroOrNegative_ReturnsError_WhenProvided()
    {
        var command = ValidCommand();
        command.KiloCalories = 0;

        var result = _sut.Validate(command);

        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDishCommand.KiloCalories));
    }

    [Fact]
    public void ValidCommand_HasNoErrors()
    {
        var result = _sut.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }
}
