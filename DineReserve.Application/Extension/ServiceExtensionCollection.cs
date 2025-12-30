using DineReserve.Application.Restaurants;
using DineReserve.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace DineReserve.Application.Extension
{
    public static class ServiceExtensionCollection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddAutoMapper(typeof(ServiceExtensionCollection).Assembly);
        }
    }
}
