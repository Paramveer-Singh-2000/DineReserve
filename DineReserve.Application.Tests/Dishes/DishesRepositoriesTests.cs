using DineReserve.Domain.Entities;
using DineReserve.Infrastructure.Persistance;
using DineReserve.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DineReserve.Application.Tests.Dishes;

public class DishesRepositoriesTests
{
    private static DineReserveDbContext NewContext() =>
        new(new DbContextOptionsBuilder<DineReserveDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Create_PersistsDish_AndReturnsGeneratedId()
    {
        await using var context = NewContext();
        var sut = new DishesRepositories(context);
        var dish = new Dish { Name = "Ramen", Price = 12.5m, RestaurantId = 1 };

        var id = await sut.Create(dish);

        id.Should().BePositive();
        (await context.Dishes.FindAsync(id)).Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteDishAsync_RemovesOnlyTheSpecifiedDishes()
    {
        await using var context = NewContext();
        var keep = new Dish { Name = "Keep Me", RestaurantId = 1 };
        var removeA = new Dish { Name = "Remove A", RestaurantId = 1 };
        var removeB = new Dish { Name = "Remove B", RestaurantId = 1 };
        context.AddRange(keep, removeA, removeB);
        await context.SaveChangesAsync();
        var sut = new DishesRepositories(context);

        await sut.DeleteDishAsync([removeA, removeB]);

        var remaining = await context.Dishes.ToListAsync();
        remaining.Should().ContainSingle().Which.Name.Should().Be("Keep Me");
    }

    [Fact]
    public async Task DeleteDishAsync_WithEmptyCollection_DeletesNothing()
    {
        await using var context = NewContext();
        context.Add(new Dish { Name = "Untouched", RestaurantId = 1 });
        await context.SaveChangesAsync();
        var sut = new DishesRepositories(context);

        await sut.DeleteDishAsync([]);

        (await context.Dishes.CountAsync()).Should().Be(1);
    }
}
