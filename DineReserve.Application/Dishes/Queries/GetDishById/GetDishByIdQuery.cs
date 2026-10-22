using DineReserve.Application.Dishes.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Queries.GetDishById
{
    public class GetDishByIdQuery(int restaurantId, int dishId): IRequest<DishDto>
    {
        public int RestaurantId { get; set; } = restaurantId;
        public int DishId { get; set;} = dishId;
    }
}
