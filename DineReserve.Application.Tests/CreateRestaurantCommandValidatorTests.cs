
using DineReserve.Application.Restaurants.Commands.CreateRestaurant;
using FluentValidation.TestHelper;
using Xunit;

namespace DineReserve.Application.Tests;

public class CreateRestaurantCommandValidatorTests
{
    private readonly CreateRestaurantCommandValidator _validator = new();

    private static CreateRestaurantCommand ValidCommand() => new()
    {
        Name = "Test Restaurant",
        Description = "A family restaurant serving fresh food",
        Category = "Italian",
        ContactEmail = "test@example.com",
        ContactPhone = "8055551234",
        ZipCode = "12345",
        Street = "123 Main Street",
        City = "Thousand Oaks",
        State = "CA",
        Country = "USA"
    };

    [Fact]
    public void Validator_ForValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = ValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_ForInvalidCommand_ShouldHaveValidationErrors()
    {
        var command = new CreateRestaurantCommand
        {
            Name = "Te",
            Description = "",
            Category = "Ialian",
            ContactEmail = "invalid-email",
            ContactPhone = "",
            ZipCode = "1345"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldHaveValidationErrorFor(x => x.Category);
        result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
        result.ShouldHaveValidationErrorFor(x => x.ContactPhone);
        result.ShouldHaveValidationErrorFor(x => x.ZipCode);
    }

    [Theory]
    [InlineData("Italian")]
    [InlineData("Chinese")]
    [InlineData("Mexican")]
    [InlineData("Indian")]
    public void Validator_ForValidCategory_ShouldNotHaveValidationErrors(
        string category)
    {
        var command = ValidCommand();
        command.Category = category;

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Te")]
    public void Validator_ForInvalidName_ShouldHaveValidationErrors(
        string name)
    {
        var command = ValidCommand();
        command.Name = name;

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validator_ForMissingDescription_ShouldHaveValidationError()
    {
        var command = ValidCommand();
        command.Description = "";

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validator_ForMissingContactPhone_ShouldHaveValidationError()
    {
        var command = ValidCommand();
        command.ContactPhone = "";

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContactPhone);
    }
}
