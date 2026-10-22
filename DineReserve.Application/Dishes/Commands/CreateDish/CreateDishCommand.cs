using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Commands.CreateDish
{
    public class CreateDishCommand:IRequest<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? KiloCalories { get; set; }
        public int RestaurantId { get; set; }
    }
}
