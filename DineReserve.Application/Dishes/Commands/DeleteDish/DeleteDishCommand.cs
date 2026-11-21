using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Commands.DeleteDish
{
    public class DeleteDishCommand(int restaurantId, int dishId) : IRequest
    {
        public int RestaurantId { get; } = restaurantId;
        public int DishId { get; } = dishId;
    }
}
