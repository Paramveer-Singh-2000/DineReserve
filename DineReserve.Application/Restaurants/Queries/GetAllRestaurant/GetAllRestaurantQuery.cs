using DineReserve.Application.Restaurants.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Queries.GetAllRestaurant
{
    public class GetAllRestaurantQuery : IRequest<IEnumerable<RestaurantDto>>
    {
    }
}
