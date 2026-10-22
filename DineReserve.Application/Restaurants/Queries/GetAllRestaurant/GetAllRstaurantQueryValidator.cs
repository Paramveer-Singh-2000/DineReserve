using DineReserve.Application.Restaurants.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Queries.GetAllRestaurant
{
    public class GetAllRstaurantQueryValidator : AbstractValidator<GetAllRestaurantQuery>
    {
        private int[] allowPageSize = [5,10,15,20,30];
        private string[] allowSortinngColumn = [nameof(RestaurantDto.Description), nameof(RestaurantDto.Name), nameof(RestaurantDto.Category)];

        public GetAllRstaurantQueryValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Must(value => allowPageSize.Contains(value))
                .WithMessage($"Page Size must be in [{string.Join(", ", allowPageSize)}]");

            RuleFor(r => r.SortBy).Must(value => allowSortinngColumn.Contains(value))
                .When(x => !string.IsNullOrEmpty(x.SortBy))
                .WithMessage($"Sort By must be in [{string.Join(", ", allowSortinngColumn)}]");
            RuleFor(r => r.SortDirection).IsInEnum();
        }
    }
}
