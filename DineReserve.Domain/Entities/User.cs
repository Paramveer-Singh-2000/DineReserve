using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Domain.Entities
{
    public class User: IdentityUser
    {
        public DateOnly? DateOfBirth { get; set; }
        public string? Nationality { get; set; }

        public List<Restaurant> OwnedRestaurant { get; set; } = [];
    }
}
