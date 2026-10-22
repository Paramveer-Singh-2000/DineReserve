using AutoMapper;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Application.Users;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Commands.CreateRestaurant
{
    public class CreateRestaurantHandler(IUserContext userContext, ILogger<CreateRestaurantHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories) : IRequestHandler<CreateRestaurantCommand, int>
    {
        public async Task<int> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            logger.LogInformation("{UserEmail} [{UserId}] is creating a new restaurant {@Restaurant}",
            currentUser.Email,
            currentUser.UserId,
            request);
            var restaurant = mapper.Map<Restaurant>(request);
            restaurant.OwnerId = currentUser.UserId;
            var id = await restaurantRepositories.CreateRestaurantAsync(restaurant);
            return id;
        }
    }
}
