using DineReserve.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Domain.Repositories
{
    public interface IRestaurantRepositories
    {
        Task<IEnumerable<Restaurant>> GetAllAsync();
        Task<Restaurant?> GetRestaurantByIdAsync(int Id);
        Task<int> CreateRestaurantAsync(Restaurant restaurant);
    }
}
