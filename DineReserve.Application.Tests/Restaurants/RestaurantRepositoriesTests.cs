using DineReserve.Domain.Constants;
using DineReserve.Domain.Entities;
using DineReserve.Infrastructure.Persistance;
using DineReserve.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

public class RestaurantRepositoriesTests
{
    private static DineReserveDbContext NewContext() =>
        new(new DbContextOptionsBuilder<DineReserveDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task CreateRestaurantAsync_PersistsAndReturnsGeneratedId()
    {
        await using var context = NewContext();
        var sut = new RestaurantRepositories(context);
        var restaurant = new Restaurant { Name = "Sushi Place", Description = "desc", Category = "Japanese" };

        var id = await sut.CreateRestaurantAsync(restaurant);

        id.Should().BePositive();
        (await context.Restaurants.FindAsync(id)).Should().NotBeNull();
    }

    [Fact]
    public async Task GetRestaurantByIdAsync_ReturnsNull_WhenNotFound()
    {
        await using var context = NewContext();
        var sut = new RestaurantRepositories(context);

        var result = await sut.GetRestaurantByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetRestaurantByIdAsync_IncludesDishes()
    {
        await using var context = NewContext();
        var restaurant = new Restaurant { Name = "Pizza Place", Description = "desc", Category = "Italian" };
        context.Add(restaurant);
        await context.SaveChangesAsync();
        context.Add(new Dish { Name = "Margherita", RestaurantId = restaurant.Id });
        await context.SaveChangesAsync();
        var sut = new RestaurantRepositories(context);

        var result = await sut.GetRestaurantByIdAsync(restaurant.Id);

        result.Should().NotBeNull();
        result!.Dishes.Should().ContainSingle(d => d.Name == "Margherita");
    }

    [Fact]
    public async Task DeleteRestaurantAsync_RemovesIt()
    {
        await using var context = NewContext();
        var restaurant = new Restaurant { Name = "Closing Down", Description = "desc", Category = "American" };
        context.Add(restaurant);
        await context.SaveChangesAsync();
        var sut = new RestaurantRepositories(context);

        await sut.DeleteRestaurantAsync(restaurant);

        (await context.FindAsync(restaurant.Id)).Should().BeNull();
    }

    [Fact]
    public async Task GetAllMatchingAsync_FiltersBySearchPhrase_AcrossNameAndDescription()
    {
        await using var context = NewContext();
        context.AddRange(
            new Restaurant { Name = "Taco Town", Description = "Best tacos in the city", Category = "Mexican" },
            new Restaurant { Name = "Burger Barn", Description = "Home of the smash burger", Category = "American" },
            new Restaurant { Name = "Pizza Palace", Description = "Serving tacos on Tuesdays too", Category = "Italian" });
        await context.SaveChangesAsync();
        var sut = new RestaurantRepositories(context);

        var (results, totalCount) = await sut.GetAllMatchingAsync(
            searchPhrase: "taco", pageSize: 10, pageNumber: 1, sortBy: null, sortDirection: SortDirection.Ascending);

        totalCount.Should().Be(2, "the phrase 'taco' appears in the name of one restaurant and the description of another");
        results.Select(r => r.Name).Should().BeEquivalentTo(["Taco Town", "Pizza Palace"]);
    }

    [Theory]
    [InlineData(SortDirection.Ascending, "Apple Bistro")]
    [InlineData(SortDirection.Descending, "Zebra Diner")]
    public async Task GetAllMatchingAsync_SortsByRequestedColumnAndDirection(SortDirection direction, string expectedFirstName)
    {
        await using var context = NewContext();
        context.AddRange(
            new Restaurant { Name = "Zebra Diner", Description = "d", Category = "American" },
            new Restaurant { Name = "Middle Cafe", Description = "d", Category = "American" },
            new Restaurant { Name = "Apple Bistro", Description = "d", Category = "American" });
        await context.SaveChangesAsync();
        var sut = new RestaurantRepositories(context);

        var (results, _) = await sut.GetAllMatchingAsync(
            searchPhrase: null, pageSize: 10, pageNumber: 1, sortBy: nameof(Restaurant.Name), sortDirection: direction);

        results.First().Name.Should().Be(expectedFirstName);
    }

    [Fact]
    public async Task GetAllMatchingAsync_SecondPage_SkipsFirstPagesResults()
    {
        await using var context = NewContext();
        for (var i = 1; i <= 25; i++)
        {
            context.Add(new Restaurant { Name = $"Restaurant {i:D2}", Description = "d", Category = "American" });
        }
        await context.SaveChangesAsync();
        var sut = new RestaurantRepositories(context);

        var (page1, _) = await sut.GetAllMatchingAsync(null, pageSize: 10, pageNumber: 1, sortBy: nameof(Restaurant.Name), sortDirection: SortDirection.Ascending);
        var (page2, _) = await sut.GetAllMatchingAsync(null, pageSize: 10, pageNumber: 2, sortBy: nameof(Restaurant.Name), sortDirection: SortDirection.Ascending);

        page1.Select(r => r.Id).Should().NotIntersectWith(page2.Select(r => r.Id));
        page2.Should().HaveCount(10);
    }
}
