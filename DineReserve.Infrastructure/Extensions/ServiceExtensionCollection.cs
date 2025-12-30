using DineReserve.Domain.Repositories;
using DineReserve.Infrastructure.Persistance;
using DineReserve.Infrastructure.Repositories;
using DineReserve.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DineReserve.Infrastructure.Extensions;

public static class ServiceExtensionCollection
{
     public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
     {
          var connectionString = configuration.GetConnectionString("DineReserveDb");
          services.AddDbContext<DineReserveDbContext>(options => options.UseSqlServer(connectionString));

          services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
          services.AddScoped<IRestaurantRepositories, RestaurantRepositories>();
     }
}