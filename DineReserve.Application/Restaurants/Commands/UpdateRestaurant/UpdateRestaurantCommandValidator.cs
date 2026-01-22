using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Commands.UpdateRestaurant
{
    public class UpdateRestaurantCommandValidator: AbstractValidator<UpdateRestaurantCommand>
    {
        public UpdateRestaurantCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().Length(3, 100).WithMessage("Restaurant name must be between 3 and 100 characters.");

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        }
    }
}
