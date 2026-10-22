using AutoMapper;
using DineReserve.Application.Restaurants;

namespace DineReserve.Application.Tests.TestHelpers;

/// <summary>
/// Builds a real AutoMapper IMapper from every Profile in the Application assembly
/// (including internal ones, like DishesProfile) - the same way production DI does
/// via services.AddAutoMapper(assembly), just without a DI container.
/// </summary>
public static class MapperTestHelper
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(RestaurantProfile).Assembly));
        return config.CreateMapper();
    }
}
