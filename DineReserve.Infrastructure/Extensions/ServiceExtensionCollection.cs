using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Repositories;
using DineReserve.Infrastructure.Authorization;
using DineReserve.Infrastructure.Authorization.Requirements;
using DineReserve.Infrastructure.Repositories;
using DineReserve.Infrastructure.Seeders;
using DineReserve.Domain;
using DineReserve.Infrastructure.Authorization.Services;
using DineReserve.Infrastructure.Persistance;
using DineReserve.Domain.Services;

namespace DineReserve.Infrastructure.Extensions;

public static class ServiceExtensionCollection
{
     public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
     {
          var connectionString = configuration.GetConnectionString("DineReserveDb");
          services.AddDbContext<DineReserveDbContext>(options => options.UseSqlServer(connectionString));

          services.AddAuthorizationBuilder().AddPolicy(PolicyNames.HasNationality, builder => builder.RequireClaim("Nationality","India","USA"))
            .AddPolicy(PolicyNames.AtLeast20, builder => builder.AddRequirements(new MinimumAgeRequirement(20))).AddPolicy(PolicyNames.CreatedAtLeast2Restaurant, builder => builder.AddRequirements(new CreateMultipleRestaurantRequirement(2)));
          services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
          services.AddScoped<IRestaurantRepositories, RestaurantRepositories>();
          services.AddScoped<IDishesRepositories, DishesRepositories>();
        services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>(); 
        services.AddScoped<IAuthorizationHandler, CreateMultipleRestaurantRequirementHandler>(); 
        services.AddScoped<IRestaurantAuthorizationService, RestaurantAuthorizationService>();    
     }
}