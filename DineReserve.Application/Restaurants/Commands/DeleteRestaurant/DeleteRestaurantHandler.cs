using DineReserve.Application.Commands.DeleteRestaurant;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Commands.DeleteRestaurant
{
    public class DeleteRestaurantHandler(ILogger<DeleteRestaurantHandler> logger, IRestaurantRepositories restaurantRepositories) : IRequestHandler<DeleteRestaurantCommand>
    {
        public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting a Restaurant with Id: {RestaurantId}", request.Id);
            var restaurant = await restaurantRepositories.GetRestaurantByIdAsync(request.Id);
            if (restaurant is null) 
            {
                throw new NotFoundException($"Restaurant with {request.Id} doesn't exist");
            }
            await restaurantRepositories.DeleteRestaurantAsync(restaurant);
        }
    }
}
