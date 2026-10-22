using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Infrastructure.Authorization.Requirements
{
    public class CreateMultipleRestaurantRequirement(int minimumRestaurantCreated) : IAuthorizationRequirement
    {
        public int MinimumRestaurantCreated { get; } = minimumRestaurantCreated;
    }
}
