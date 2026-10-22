using DineReserve.Application.Restaurants.Commands.CreateRestaurant;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Tests
{
    public class CreateRestaurantCommandValidatorTests
    {
        [Fact]
        public void Validator_ForValidCommand_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var command = new CreateRestaurantCommand
            {
                Name = "Test Restaurant",
                Category = "Italian",
                ZipCode = "12345",
                ContactEmail = "test@example.com "
            };
            var validator = new CreateRestaurantCommandValidator();
            // Act
            var result = validator.TestValidate(command);
            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validator_ForInvalidCommand_ShouldHaveValidationErrors ()
        {
            // Arrange
            var command = new CreateRestaurantCommand
            {
                Name = "Tes",
                Category = "Ialian",
                ZipCode = "1345",
                ContactEmail = "testmple.com "
            };
            var validator = new CreateRestaurantCommandValidator();
            // Act
            var result = validator.TestValidate(command);
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Category);
            result.ShouldHaveValidationErrorFor(x => x.ZipCode);
            result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
        }

        [Theory]
        [InlineData("Italian")]
        [InlineData("Chinese")]
        [InlineData("Mexican")]
        [InlineData("Indian")]
        public void Validator_ForValidCategory_ShouldNotHaveValidationErrors(string category)
        {
            // Arrange
            var command = new CreateRestaurantCommand
            {
                Name = "Test Restaurant",
                Category = category,
                ZipCode = "12345",
                ContactEmail = "test@example.com"
            };
            var validator = new CreateRestaurantCommandValidator();
            // Act
            var result = validator.TestValidate(command);
            // Assert
            result.ShouldNotHaveAnyValidationErrors();
          }
    }
}
