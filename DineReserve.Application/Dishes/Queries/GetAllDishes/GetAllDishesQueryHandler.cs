using AutoMapper;
using DineReserve.Application.Dishes.Dtos;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Application.Restaurants.Queries.GetAllRestaurant;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Queries.GetAllDishes
{
    public class GetAllDishesQueryHandler(ILogger<GetAllDishesQueryHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories) : IRequestHandler<GetAllDishesQuery, IEnumerable<DishDto>>
    {
        public async Task<IEnumerable<DishDto>> Handle(GetAllDishesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieve Dishes for restaurant with Id : {restaurantId}", request.RestaurantId);
            var restaurant = await restaurantRepositories.GetRestaurantByIdAsync(request.RestaurantId);

            if(restaurant == null)
            {
                logger.LogWarning("Restaurant with Id : {restaurantId} not found", request.RestaurantId);
                throw new NotFoundException($"Restaurant with Id : {request.RestaurantId} not found");
            }

            var dishDtos = mapper.Map<IEnumerable<DishDto>>(restaurant.Dishes);
            return dishDtos;
        }

    }
}
