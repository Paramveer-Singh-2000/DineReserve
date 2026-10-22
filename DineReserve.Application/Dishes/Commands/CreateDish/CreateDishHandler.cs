using AutoMapper;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Commands.CreateDish
{
    public class CreateDishHandler(ILogger<CreateDishHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories, IDishesRepositories dishesRepositories) : IRequestHandler<CreateDishCommand,int>
    {
        public async Task<int> Handle(CreateDishCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating new dish: {@DishRequest}", request);
            var restaurant = await restaurantRepositories.GetRestaurantByIdAsync(request.RestaurantId);
            if(restaurant is null)
            {
                logger.LogWarning("Restaurant with id {RestaurantId} not found", request.RestaurantId);
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }
            var dish = mapper.Map<Dish>(request);
            return await dishesRepositories.Create(dish);
        }
    }
}
