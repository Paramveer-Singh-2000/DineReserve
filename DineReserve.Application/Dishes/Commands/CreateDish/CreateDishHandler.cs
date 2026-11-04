
using AutoMapper;
using DineReserve.Domain.Authorization;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using DineReserve.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DineReserve.Application.Dishes.Commands.CreateDish;

public class CreateDishHandler(
    ILogger<CreateDishHandler> logger,
    IMapper mapper,
    IRestaurantRepositories restaurantRepositories,
    IDishesRepositories dishesRepositories,
    IRestaurantAuthorizationService authorizationService)
    : IRequestHandler<CreateDishCommand, int>
{
    public async Task<int> Handle(
        CreateDishCommand request,
        CancellationToken cancellationToken)
    {
        var restaurant = await restaurantRepositories
            .GetRestaurantByIdAsync(request.RestaurantId);

        if (restaurant is null)
        {
            throw new NotFoundException(
                nameof(Restaurant),
                request.RestaurantId.ToString());
        }

        if (!authorizationService.Authorize(
            restaurant, ResourceOperations.Update))
        {
            throw new ForbidException(
                "You cannot add dishes to this restaurant.");
        }

        var dish = mapper.Map<Dish>(request);
        dish.RestaurantId = restaurant.Id;

        var id = await dishesRepositories.Create(dish);

        logger.LogInformation(
            "Created dish {DishId} for restaurant {RestaurantId}",
            id, restaurant.Id);

        return id;
    }
}
