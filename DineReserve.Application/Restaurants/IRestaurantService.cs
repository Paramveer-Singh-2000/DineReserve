using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Domain.Entities;

namespace DineReserve.Application.Restaurants
{
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDto>> GetAllRestaurants();
        Task<RestaurantDto?> GetRestaurantById(int Id);
    }
}