using DineReserve.Application.Dishes.Dtos;
using DineReserve.Application.Restaurants.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Queries.GetAllDishes
{
    public class GetAllDishesQuery(int restaurantId) : IRequest<IEnumerable<DishDto>>
    {
        public int RestaurantId { get; } = restaurantId;
    }
}
