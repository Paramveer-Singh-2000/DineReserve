using DineReserve.Application.Dishes.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Dtos
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivery { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public List<DishDto> Dishes { get; set; } = new();
    }
}
