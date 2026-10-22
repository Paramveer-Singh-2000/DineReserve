using DineReserve.Application.Restaurants.Commands.CreateRestaurant;
using FluentAssertions;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

/// <summary>
/// Regression tests for two real bugs found in the original validator:
/// 1. The ZipCode regex was the literal string "@\"^\d{2}(-\d{3})?$\"" (quotes and @ baked
///    into the pattern), so it could never match a real zip code.
/// 2. The Category rule's error message read "Address is required." (copy-pasted).
/// </summary>
public class CreateRestaurantCommandValidatorTests
{
    private readonly CreateRestaurantCommandValidator _sut = new();

    private static CreateRestaurantCommand ValidCommand() => new()
    {
        Name = "Valid Restaurant Name",
        Description = "A valid description",
        Category = "Italian",
        ContactEmail = "contact@restaurant.com",
        ContactPhone = "555-0100",
        ZipCode = "90210"
    };

    [Theory]
    [InlineData("90210")]
    [InlineData("90210-1234")]
    public void ZipCode_AcceptsRealZipCodes(string zip)
    {
        var command = ValidCommand();
        command.ZipCode = zip;

        var result = _sut.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateRestaurantCommand.ZipCode));
    }

    [Fact]
    public void ZipCode_RejectsObviouslyInvalidValue()
    {
        var command = ValidCommand();
        command.ZipCode = "not-a-zip";

        var result = _sut.Validate(command);

        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateRestaurantCommand.ZipCode));
    }

    [Fact]
    public void Category_ErrorMessage_ReferencesCategory_NotAddress()
    {
        var command = ValidCommand();
        command.Category = "NotAllowedCategory";

        var result = _sut.Validate(command);
        var categoryErrors = result.Errors.Where(e => e.PropertyName == nameof(CreateRestaurantCommand.Category));

        categoryErrors.Should().NotBeEmpty();
        categoryErrors.Should().NotContain(e => e.ErrorMessage.Contains("Address"));
    }
}
