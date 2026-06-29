using DineReserve.Application.Restaurants.Commands.DeleteRestaurant;
using DineReserve.Domain;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using DineReserve.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Commands.DeleteRestaurant
{
    public class DeleteDishQueryHandler(ILogger<DeleteRestaurantHandler> logger, IRestaurantRepositories restaurantRepositories, IRestaurantAuthorizationService restaurantAuthorizationService) : IRequestHandler<DeleteDishCommand>
    {
        public async Task Handle(DeleteDishCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting a Restaurant with Id: {RestaurantId}", request.Id);
            var restaurant = await restaurantRepositories.GetRestaurantByIdAsync(request.Id);
            if (restaurant is null)
            {
                throw new NotFoundException(nameof(Restaurant), request.Id.ToString());
            }
            if (!restaurantAuthorizationService.Authorize(restaurant, Domain.Authorization.ResourceOperations.Delete))
                throw new ForbidException();
            await restaurantRepositories.DeleteRestaurantAsync(restaurant);
        }
    }
}
