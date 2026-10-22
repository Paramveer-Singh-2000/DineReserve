using DineReserve.Application.Users;
using DineReserve.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Infrastructure.Authorization.Requirements
{
    internal class CreateMultipleRestaurantRequirementHandler(IRestaurantRepositories restaurantRepositories, IUserContext userContext) : AuthorizationHandler<CreateMultipleRestaurantRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateMultipleRestaurantRequirement requirement)
        {
            var currentUser = userContext.GetCurrentUser();
            var restaurants = await restaurantRepositories.GetAllAsync();
            var userRestaurantsCount = restaurants.Count(r => r.OwnerId == currentUser!.UserId);
            if (userRestaurantsCount >= requirement.MinimumRestaurantCreated)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
