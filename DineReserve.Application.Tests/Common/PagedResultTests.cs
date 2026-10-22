using DineReserve.Application.Common;
using FluentAssertions;
using Xunit;

namespace DineReserve.Application.Tests.Common;

/// <summary>
/// Regression tests for a real bug found in the original code:
/// TotalPages used integer division (totalCount / pageSize) with no rounding up,
/// so e.g. 10 items at 3 per page reported 3 pages instead of the correct 4.
/// </summary>
public class PagedResultTests
{
    [Theory]
    [InlineData(10, 3, 4)]   // 3.33 -> should round up to 4 pages
    [InlineData(9, 3, 3)]    // exact division still works
    [InlineData(1, 5, 1)]    // fewer items than one page
    [InlineData(0, 5, 0)]    // no items at all
    public void TotalPages_RoundsUpToNearestWholePage(int totalCount, int pageSize, int expectedPages)
    {
        var result = new PagedResult<string>([], totalCount, pageSize, pageNumber: 1);

        result.TotalPages.Should().Be(expectedPages);
    }

    [Fact]
    public void Constructor_DoesNotThrow_WhenPageSizeIsZero()
    {
        var act = () => new PagedResult<string>([], totalCount: 10, pageSize: 0, pageNumber: 1);

        act.Should().NotThrow<DivideByZeroException>();
    }

    [Fact]
    public void ItemFrom_And_ItemTo_AreOneIndexed_AndCappedAtTotalCount()
    {
        var result = new PagedResult<string>([], totalCount: 12, pageSize: 5, pageNumber: 3);

        result.ItemFrom.Should().Be(11);
        result.ItemTo.Should().Be(12); // capped, not 15, since there are only 12 items total
    }
}
