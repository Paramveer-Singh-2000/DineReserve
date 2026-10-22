using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Commands.CreateDish
{
    internal class CreateDishCommandValidator :AbstractValidator<CreateDishCommand>
    {
        public CreateDishCommandValidator()
        {
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.KiloCalories)
                .GreaterThan(0).WithMessage("KiloCalories must be greater than zero.");

        }
    }
}
