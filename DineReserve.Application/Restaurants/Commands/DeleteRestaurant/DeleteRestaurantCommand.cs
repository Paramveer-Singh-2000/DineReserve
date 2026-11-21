using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Commands.DeleteRestaurant
{
    public class DeleteRestaurantCommand(int restaurantId) : IRequest
    {
            public int Id { get; set; } = restaurantId;

    }
}
