using DineReserve.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Domain.Repositories
{
    public interface IDishesRepositories
    {
        Task<int> Create(Dish entity);
        Task DeleteDishAsync(IEnumerable<Dish> dishes);
    }
}
