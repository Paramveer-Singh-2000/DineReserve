using AutoMapper;
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

namespace DineReserve.Application.Restaurants.Commands.UpdateRestaurant
{
    internal class UpdateRestaurantHandler(ILogger<UpdateRestaurantHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories, IRestaurantAuthorizationService restaurantAuthorizationService) : IRequestHandler<UpdateRestaurantCommand>
    {
        public async Task Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating a Restaurant with Id: {RestaurantId} with {@UpdatedRestaurant}", request.Id, request);
            var restaurant = await restaurantRepositories.GetRestaurantByIdAsync(request.Id);
            if (restaurant is null)
            {
                throw new NotFoundException(nameof(Restaurant), request.Id.ToString());
            }
            if (!restaurantAuthorizationService.Authorize(restaurant, Domain.Authorization.ResourceOperations.Delete))
                throw new ForbidException();

            mapper.Map(request, restaurant);
            await restaurantRepositories.SaveChanges();
        }
    }
}
