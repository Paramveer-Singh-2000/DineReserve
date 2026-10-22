using DineReserve.Domain.Constants;
using DineReserve.Domain.Entities;
using DineReserve.Infrastructure.Persistance;
using DineReserve.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

/// <summary>
/// Regression test for a real bug found in the original code: GetAllMatchingAsync used a
/// hardcoded .Take(5) instead of .Take(pageSize), so requesting any page size other than 5
/// silently returned the wrong number of results.
/// </summary>
public class RestaurantRepositoriesPaginationTests
{
    private static DineReserveDbContext CreateContextWithRestaurants(int count)
    {
        var options = new DbContextOptionsBuilder<DineReserveDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new DineReserveDbContext(options);
        for (var i = 0; i < count; i++)
        {
            context.Add(new Restaurant
            {
                Name = $"Restaurant {i}",
                Description = "desc",
                Category = "Italian"
            });
        }
        context.SaveChanges();
        return context;
    }

    [Theory]
    [InlineData(10, 30)]
    [InlineData(10, 15)]
    [InlineData(10, 20)]
    public async Task GetAllMatchingAsync_ReturnsRequestedPageSize_NotHardcodedFive(int totalRestaurants, int pageSize)
    {
        await using var context = CreateContextWithRestaurants(totalRestaurants);
        var sut = new RestaurantRepositories(context);

        var (results, totalCount) = await sut.GetAllMatchingAsync(
            searchPhrase: null, pageSize: pageSize, pageNumber: 1, sortBy: null, sortDirection: SortDirection.Ascending);

        totalCount.Should().Be(totalRestaurants);
        results.Should().HaveCount(Math.Min(pageSize, totalRestaurants));
    }
}
