
using FluentValidation;

namespace DineReserve.Application.Dishes.Commands.CreateDish;

internal class CreateDishCommandValidator
    : AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Dish name is required.")
            .MaximumLength(100)
            .WithMessage(
                "Dish name cannot exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage(
                "Price must be greater than zero.");

        RuleFor(x => x.KiloCalories)
            .GreaterThan(0)
            .When(x => x.KiloCalories.HasValue)
            .WithMessage(
                "Calories must be greater than zero when provided.");
    }
}
