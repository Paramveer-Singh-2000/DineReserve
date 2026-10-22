using AutoMapper;
using DineReserve.Application.Restaurants.Commands.CreateRestaurant;
using DineReserve.Application.Restaurants.Commands.UpdateRestaurant;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants
{
    public class RestaurantProfile:Profile
    {
        public RestaurantProfile()
        {
            CreateMap<UpdateRestaurantCommand, Restaurant>();
        CreateMap<CreateRestaurantCommand, Restaurant>().ForMember(d => d.Address, opt => opt.MapFrom(src => new Address
        {
            City = src.City,
            Country = src.Country,
            State = src.State,
            Street = src.Street,
            ZipCode = src.ZipCode,
        }));

         CreateMap<Restaurant,RestaurantDto>()
                .ForMember(d => d.Street, opt => opt.MapFrom(src => src.Address == null ? null : src.Address.Street))
                .ForMember(d => d.City, opt => opt.MapFrom(src => src.Address == null ? null : src.Address.City))
                .ForMember(d => d.ZipCode, opt => opt.MapFrom(src => src.Address == null ? null : src.Address.ZipCode))
                .ForMember(d => d.Country, opt => opt.MapFrom(src => src.Address == null ? null : src.Address.Country))
                .ForMember(d => d.State, opt => opt.MapFrom(src => src.Address == null ? null : src.Address.State))
                .ForMember(d => d.Dishes, opt => opt.MapFrom(src => src.Dishes == null ? null : src.Dishes))
                ; 
        }
    }
}
