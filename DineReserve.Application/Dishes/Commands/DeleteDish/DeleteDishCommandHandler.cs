using DineReserve.Application.Restaurants.Commands.DeleteRestaurant;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Commands.DeleteRestaurant
{
    public class DeleteDishQueryHandler(ILogger<DeleteRestaurantHandler> logger, IRestaurantRepositories restaurantRepositories) : IRequestHandler<DeleteDishCommand>
    {
        public async Task Handle(DeleteDishCommand request, CancellationToken cancellationToken)
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
