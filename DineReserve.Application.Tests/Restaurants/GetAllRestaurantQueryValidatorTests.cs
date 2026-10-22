using DineReserve.Application.Restaurants.Queries.GetAllRestaurant;
using DineReserve.Domain.Constants;
using FluentAssertions;
using Xunit;

namespace DineReserve.Application.Tests.Restaurants;

public class GetAllRestaurantQueryValidatorTests
{
    private readonly GetAllRestaurantQueryValidator _sut = new();

    private static GetAllRestaurantQuery ValidQuery() => new()
    {
        PageNumber = 1,
        PageSize = 10,
        SortBy = null,
        SortDirection = SortDirection.Ascending
    };

    [Fact]
    public void ValidQuery_HasNoErrors()
    {
        var result = _sut.Validate(ValidQuery());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageNumber_Rejects_LessThanOne(int pageNumber)
    {
        var query = ValidQuery();
        query.PageNumber = pageNumber;

        var result = _sut.Validate(query);

        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetAllRestaurantQuery.PageNumber));
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    [InlineData(30)]
    public void PageSize_AcceptsOnlyTheAllowedValues(int pageSize)
    {
        var query = ValidQuery();
        query.PageSize = pageSize;

        var result = _sut.Validate(query);

        result.Errors.Should().NotContain(e => e.PropertyName == nameof(GetAllRestaurantQuery.PageSize));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(100)]
    public void PageSize_RejectsAnyOtherValue(int pageSize)
    {
        var query = ValidQuery();
        query.PageSize = pageSize;

        var result = _sut.Validate(query);

        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetAllRestaurantQuery.PageSize));
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("Description")]
    [InlineData("Category")]
    public void SortBy_AcceptsKnownRestaurantDtoColumns(string sortBy)
    {
        var query = ValidQuery();
        query.SortBy = sortBy;

        var result = _sut.Validate(query);

        result.Errors.Should().NotContain(e => e.PropertyName == nameof(GetAllRestaurantQuery.SortBy));
    }

    [Fact]
    public void SortBy_Rejects_UnknownColumn()
    {
        var query = ValidQuery();
        query.SortBy = "SomeColumnThatDoesNotExist";

        var result = _sut.Validate(query);

        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetAllRestaurantQuery.SortBy));
    }

    [Fact]
    public void SortBy_IsOptional_AndSkipsValidationWhenNull()
    {
        var query = ValidQuery();
        query.SortBy = null;

        var result = _sut.Validate(query);

        result.Errors.Should().NotContain(e => e.PropertyName == nameof(GetAllRestaurantQuery.SortBy));
    }

    [Fact]
    public void SortDirection_Rejects_OutOfRangeEnumValue()
    {
        var query = ValidQuery();
        query.SortDirection = (SortDirection)99;

        var result = _sut.Validate(query);

        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetAllRestaurantQuery.SortDirection));
    }
}
