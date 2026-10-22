using DineReserve.Application.Common;
using DineReserve.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Domain.Repositories
{
    public interface IRestaurantRepositories
    {
        Task<IEnumerable<Restaurant>> GetAllAsync();
        Task<(IEnumerable<Restaurant>, int)>  GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection);
        Task<Restaurant?> GetRestaurantByIdAsync(int Id);
        Task<int> CreateRestaurantAsync(Restaurant restaurant);
        Task DeleteRestaurantAsync(Restaurant restaurant);
        Task SaveChanges();
    }
}
