using DineReserve.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DineReserve.Infrastructure.Authorization
{
    public class RestaurantUserClaimFactoryPrincipal(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<User, IdentityRole>(userManager, roleManager, options)
    {
        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var id = await GenerateClaimsAsync(user);
            if (user.Nationality is not null)
            {
                id.AddClaim(new Claim(AppClaimsTypes.Nationality, user.Nationality));
            }
            if (user.DateOfBirth is not null)
            {
                id.AddClaim(new Claim(AppClaimsTypes.DateOfBirth, user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
            }
            return new ClaimsPrincipal(id);
        }
    }
}
