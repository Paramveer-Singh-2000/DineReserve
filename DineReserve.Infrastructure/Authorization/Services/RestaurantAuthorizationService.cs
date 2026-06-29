using DineReserve.Application.Users;
using DineReserve.Domain.Services;
using Microsoft.Extensions.Logging;
using DineReserve.Domain.Constants;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Authorization;


namespace DineReserve.Infrastructure.Authorization.Services;

public class RestaurantAuthorizationService(ILogger<RestaurantAuthorizationService> logger,
    IUserContext userContext) : IRestaurantAuthorizationService
{
    public bool Authorize(Restaurant restaurant, ResourceOperations resourceOperations)
    {
        var user = userContext.GetCurrentUser();

        logger.LogInformation("Authorizing user {UserEmail}, to {Operation} for restaurant {RestaurantName}", user.Email, resourceOperations, restaurant.Name);

        if (resourceOperations == ResourceOperations.Create || resourceOperations == ResourceOperations.Read)
        {
            logger.LogInformation("Operation Read/Create succesfully authorization");
            return true;
        }
        if (resourceOperations == ResourceOperations.Delete && user.IsInRole(UserRoles.Admin))
        {
            logger.LogInformation("Admin User Operation Delete succesfully authorization");
            return true;
        }
        if (resourceOperations == ResourceOperations.Delete || resourceOperations == ResourceOperations.Update && user.UserId == restaurant.OwnerId)
        {
            logger.LogInformation("Owner User succesfully authorization");
            return true;
        }
        return false;
    }
}
