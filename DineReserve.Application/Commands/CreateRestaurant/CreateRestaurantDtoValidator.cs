using DineReserve.Application.Restaurants.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Commands.CreateRestaurant
{
    public class CreateRestaurantCommandValidator :AbstractValidator<CreateRestaurantCommand>
    {
        readonly List<string> allowedCatagories = new() { "Italian", "Chinese", "Indian", "Mexican", "American", "French", "Japanese" };
        public CreateRestaurantCommandValidator()
        {
            RuleFor (x => x.Name)
                .NotEmpty().Length(3,100).WithMessage("Restaurant name must be between 3 and 100 characters.");

            RuleFor (x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Address is required.").Must(x => allowedCatagories.Contains(x));
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().WithMessage("A valid contact email is required.");
            RuleFor(x => x.ContactPhone).NotEmpty().WithMessage("Contact number is required.");
            RuleFor(x => x.ZipCode).Matches("@\"^\\d{2}(-\\d{3})?$\"").WithMessage("Enter a valid zip code");



        }
    }
}
