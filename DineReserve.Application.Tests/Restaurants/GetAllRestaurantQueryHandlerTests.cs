using DineReserve.Application.Restaurants.Queries.GetAllRestaurant;
using DineReserve.Application.Tests.TestHelpers;
using DineReserve.Domain.Constants;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

public class GetAllRestaurantQueryHandlerTests
{
    private readonly IRestaurantRepositories _restaurantRepositories = Substitute.For<IRestaurantRepositories>();
    private readonly GetAllRestaurantQueryHandler _sut;

    public GetAllRestaurantQueryHandlerTests()
    {
        _sut = new GetAllRestaurantQueryHandler(
            Substitute.For<ILogger<GetAllRestaurantQueryHandler>>(),
            MapperTestHelper.CreateMapper(),
            _restaurantRepositories);
    }

    [Fact]
    public async Task Handle_PassesAllQueryParameters_ToRepository()
    {
        _restaurantRepositories
            .GetAllMatchingAsync(Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<SortDirection>())
            .Returns((new List<Restaurant>(), 0));

        var query = new GetAllRestaurantQuery
        {
            SearchPhrases = "pizza",
            PageSize = 10,
            PageNumber = 2,
            SortBy = "Name",
            SortDirection = SortDirection.Descending
        };

        await _sut.Handle(query, CancellationToken.None);

        await _restaurantRepositories.Received(1).GetAllMatchingAsync("pizza", 10, 2, "Name", SortDirection.Descending);
    }

    [Fact]
    public async Task Handle_BuildsPagedResult_FromRepositoryTotalCount()
    {
        var restaurants = new List<Restaurant>
        {
            new() { Id = 1, Name = "A", Description = "d", Category = "Italian" },
            new() { Id = 2, Name = "B", Description = "d", Category = "Mexican" }
        };
        _restaurantRepositories
            .GetAllMatchingAsync(null, 2, 1, null, SortDirection.Ascending)
            .Returns((restaurants, 20)); // 20 total matches, only 2 returned for this page

        var result = await _sut.Handle(new GetAllRestaurantQuery { PageSize = 2, PageNumber = 1 }, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.TotalItemCount.Should().Be(20);
        result.TotalPages.Should().Be(10);
    }
}
