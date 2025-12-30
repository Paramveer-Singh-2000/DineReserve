using AutoMapper;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants
{
    public class RestaurantService(IRestaurantRepositories restaurantRepositories, ILogger<RestaurantService> logger, IMapper mapper) : IRestaurantService
    {
        public async Task<IEnumerable<RestaurantDto>> GetAllRestaurants()
        {
            logger.LogInformation("Getting all restaurants");
            var restaurants = await restaurantRepositories.GetAllAsync();
            var restaurantDto = mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
            return restaurantDto;
        }

        public async Task<RestaurantDto?> GetRestaurantById(int Id)
        {
            logger.LogInformation("Getting all restaurants");
            var restaurants = await restaurantRepositories.GetRestaurantByIdAsync(Id);
            var restaurantDto = mapper.Map<RestaurantDto>(restaurants);
            return restaurantDto;
        }
    }
}
