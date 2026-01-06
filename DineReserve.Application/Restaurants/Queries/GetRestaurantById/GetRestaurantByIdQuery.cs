using DineReserve.Application.Restaurants.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Queries.GetRestaurantById
{
    public class GetRestaurantByIdQuery : IRequest<RestaurantDto>
    {
        public int Id { get; set; }
    }
}
