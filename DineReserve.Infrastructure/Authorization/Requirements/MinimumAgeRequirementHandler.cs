using DineReserve.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Infrastructure.Authorization.Requirements
{
    public class MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger, IUserContext userContext) : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var currentUser = userContext.GetCurrentUser();
            logger.LogInformation("User: {Email}, date of birth {DoB} - Handling MinimumAgeRequirement",
            currentUser!.Email,
            currentUser.DateOfBirth);

            if(currentUser.DateOfBirth is null)
            {
                logger.LogInformation("User: {Email} does not have a date of birth set",
                currentUser.Email);
                return Task.CompletedTask;
            }

            if (currentUser.DateOfBirth.Value.AddYears(requirement.MinimumAge) <= DateOnly.FromDateTime(DateTime.UtcNow))
            {
                logger.LogInformation("User: {Email} meets the minimum age requirement of {MinimumAge}",
                currentUser.Email,
                requirement.MinimumAge);
                context.Succeed(requirement);
            }
            else
            {
                logger.LogInformation("User: {Email} does not meet the minimum age requirement of {MinimumAge}",
                currentUser.Email,
                requirement.MinimumAge);
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
