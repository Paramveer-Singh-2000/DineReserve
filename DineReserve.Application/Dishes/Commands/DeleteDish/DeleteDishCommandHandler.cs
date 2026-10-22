using DineReserve.Application.Dishes.Commands.DeleteRestaurant;
using DineReserve.Domain.Authorization;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using DineReserve.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DineReserve.Application.Dishes.Commands.DeleteDish
{
    public class DeleteDishCommandHandler(
        ILogger<DeleteDishCommandHandler> logger,
        IRestaurantRepositories restaurantRepositories,
        IDishesRepositories dishesRepositories,
        IRestaurantAuthorizationService restaurantAuthorizationService) : IRequestHandler<DeleteDishCommand>
    {
        public async Task Handle(DeleteDishCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting Dish with Id: {DishId} for Restaurant with Id: {RestaurantId}", request.DishId, request.RestaurantId);

            var restaurant = await restaurantRepositories.GetRestaurantByIdAsync(request.RestaurantId);
            if (restaurant is null)
            {
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }

            var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == request.DishId);
            if (dish is null)
            {
                throw new NotFoundException(nameof(Dish), request.DishId.ToString());
            }

            if (!restaurantAuthorizationService.Authorize(restaurant, ResourceOperations.Delete))
                throw new ForbidException($"You are not allowed to delete dishes from restaurant with id: {request.RestaurantId}.");

            await dishesRepositories.DeleteDishAsync([dish]);
        }
    }
}