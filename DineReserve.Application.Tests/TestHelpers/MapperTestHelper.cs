
using AutoMapper;
using DineReserve.Application.Restaurants;
using Microsoft.Extensions.Logging.Abstractions;

namespace DineReserve.Application.Tests.TestHelpers;

public static class MapperTestHelper
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddMaps(
                typeof(RestaurantProfile).Assembly),
            NullLoggerFactory.Instance);

        return config.CreateMapper();
    }
}
