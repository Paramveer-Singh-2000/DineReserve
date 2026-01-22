using AutoMapper;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Queries.GetAllRestaurant
{
    public class GetAllRestaurantQueryHandler(ILogger<GetAllRestaurantQueryHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories) : IRequestHandler<GetAllRestaurantQuery, IEnumerable<RestaurantDto>>
    {
        public async Task<IEnumerable<RestaurantDto>> Handle(GetAllRestaurantQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting all restaurants");
            var restaurants = await restaurantRepositories.GetAllAsync();
            var restaurantDto = mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
            return restaurantDto;
        }
    }
}
