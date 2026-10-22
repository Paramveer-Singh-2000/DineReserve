using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Users.Commands.UpdateUserDetail
{
    public class UpdateUserDetailCommandHandler(ILogger<UpdateUserDetailCommandHandler> logger, IUserContext userContext, IUserStore<User> userStore) : IRequestHandler<UpdateUserDetailCommand>
    {
        public async Task Handle(UpdateUserDetailCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();
            logger.LogInformation("Handling UpdateUserDetails command with {@userId} with {@request}", user!.UserId, request);
            
            var dbUser = await userStore.FindByIdAsync(user?.UserId.ToString() ?? string.Empty, cancellationToken);
            if(dbUser == null)
            {
                logger.LogWarning("User with ID {@userId} not found", user?.UserId);
                throw new NotFoundException(nameof(user), user!.UserId);
            }
            dbUser.DateOfBirth = request.DateOfBirth;
            dbUser.Nationality = request.Nationality;

            await userStore.UpdateAsync(dbUser, cancellationToken);
        }
    }
}
