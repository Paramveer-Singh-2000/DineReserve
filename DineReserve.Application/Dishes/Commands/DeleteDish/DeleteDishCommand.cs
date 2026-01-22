using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Commands.DeleteRestaurant
{
    public class DeleteDishCommand(int restaurantId):IRequest
    {
        public int Id { get; set; } = restaurantId;

    }
}
