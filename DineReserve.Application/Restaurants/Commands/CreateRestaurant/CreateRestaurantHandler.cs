using AutoMapper;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Commands.CreateRestaurant
{
    public class CreateRestaurantHandler(ILogger<CreateRestaurantHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories) : IRequestHandler<CreateRestaurantCommand, int>
    {
        public async Task<int> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating a new Restaurant {@Restaurant}",request);
            var restaurant = mapper.Map<Restaurant>(request);
            var id = await restaurantRepositories.CreateRestaurantAsync(restaurant);
            return id;
        }
    }
}
