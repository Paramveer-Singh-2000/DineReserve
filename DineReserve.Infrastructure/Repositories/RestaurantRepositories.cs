using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using DineReserve.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Infrastructure.Repositories
{
    public class RestaurantRepositories(DineReserveDbContext dbContext) : IRestaurantRepositories
    {
        public async Task<int> CreateRestaurantAsync(Restaurant restaurant)
        {
            dbContext.Add(restaurant);
            await dbContext.SaveChangesAsync();
            return restaurant.Id;
        }

        public async Task DeleteRestaurantAsync(Restaurant restaurant)
        {
            dbContext.Remove(restaurant);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            var restaurants = await dbContext.Restaurants.Include(x => x.Dishes).ToListAsync();
            return restaurants;
        }

        public async Task<Restaurant?> GetRestaurantByIdAsync(int Id)
        {
            var restaurant = await dbContext.Restaurants.Include(x => x.Dishes).FirstOrDefaultAsync(x => x .Id == Id);
            return restaurant;
        }

        public async Task SaveChanges()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
