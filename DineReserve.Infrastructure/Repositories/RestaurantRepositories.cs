using Azure.Core;
using DineReserve.Application.Common;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using DineReserve.Infrastructure.Persistance;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        public async Task<(IEnumerable<Restaurant>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection)
        {
            var phrase = searchPhrase?.ToLower();
            var baseQuery = dbContext.Restaurants.Include(x => x.Dishes)
                .Where(x => searchPhrase == null || x.Name.ToLower().Contains(phrase) || x.Description.ToLower().Contains(phrase));

            var totalCount = await baseQuery.CountAsync();

            if(sortBy != null)
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    { nameof(Restaurant.Name), r => r.Name },
                    { nameof(Restaurant.Description), r => r.Description },
                    { nameof(Restaurant.Category), r => r.Category }
                };
                var selectedColumn = columnsSelector[sortBy];
                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = await baseQuery
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return (restaurants, totalCount);
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
