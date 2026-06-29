using DineReserve.Application.Common;
using DineReserve.Application.Restaurants.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Queries.GetAllRestaurant
{
    public class GetAllRestaurantQuery : IRequest<PagedResult<RestaurantDto>>
    {
        public string? SearchPhrases { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
