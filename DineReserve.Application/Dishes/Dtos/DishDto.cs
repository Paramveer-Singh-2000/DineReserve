using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Dishes.Dtos
{
    public class DishDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? KiloCalories { get; set; }
    }
}
