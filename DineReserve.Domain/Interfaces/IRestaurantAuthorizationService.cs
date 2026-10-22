
using DineReserve.Domain.Authorization;
using DineReserve.Domain.Entities;

namespace DineReserve.Domain.Services
{
    public interface IRestaurantAuthorizationService
    {
        bool Authorize(Restaurant restaurant, ResourceOperations resourceOperations);
    }
}