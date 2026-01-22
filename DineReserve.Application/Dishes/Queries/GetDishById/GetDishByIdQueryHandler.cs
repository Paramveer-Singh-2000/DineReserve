using AutoMapper;
using DineReserve.Application.Dishes.Dtos;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Application.Restaurants.Queries.GetRestaurantById;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Queries.GetDishById
{
    public class GetDishByIdQueryHandler(ILogger<GetDishByIdQueryHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories) : IRequestHandler<GetDishByIdQuery, DishDto>
    {
        public async Task<DishDto> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting dish by Id : {dishId}", request.DishId);
            var restaurant = await restaurantRepositories.GetRestaurantByIdAsync(request.RestaurantId);

            if (restaurant == null)
            {
                logger.LogWarning("Restaurant with Id : {restaurantId} not found", request.RestaurantId);
                throw new NotFoundException($"Restaurant with Id : {request.RestaurantId} not found");
            }
            var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == request.DishId);
            var dishDtos = mapper.Map<DishDto>(dish);
            return dishDtos;
        }
    }
}
