using AutoMapper;
using DineReserve.Application.Dishes.Dtos;
using DineReserve.Domain.Entities;

namespace DineReserve.Application.Dishes
{
    internal class DishesProfile:Profile
    {
        public DishesProfile()
        {
            CreateMap<Dish, DishDto>();
        }
    }
}
