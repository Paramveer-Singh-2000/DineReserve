using AutoMapper;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Queries.GetRestaurantById
{
    public class GetRestaurantByIdQueryHandler(ILogger<GetRestaurantByIdQueryHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories) : IRequestHandler<GetRestaurantByIdQuery, RestaurantDto>
    {
        public async Task<RestaurantDto> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting all restaurants");
            var restaurants = await restaurantRepositories.GetRestaurantByIdAsync(request.Id) ?? throw new NotFoundException(nameof(Restaurant), request.Id.ToString());
            var restaurantDto = mapper.Map<RestaurantDto>(restaurants);
            return restaurantDto;
        }
    }
}
