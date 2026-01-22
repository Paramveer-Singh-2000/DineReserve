using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Commands.UpdateRestaurant
{
    public class UpdateRestaurantCommand : IRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool HasDelivery { get; set; }

    }
}
