using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using DineReserve.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Infrastructure.Repositories
{
    public class DishesRepositories(DineReserveDbContext context) : IDishesRepositories
    {
        public async Task<int> Create(Dish entity)
        {
            context.Dishes.Add(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteDishAsync(IEnumerable<Dish> dishes)
        {
            context.Dishes.RemoveRange(dishes);
            await context.SaveChangesAsync();
        }
    }
}
