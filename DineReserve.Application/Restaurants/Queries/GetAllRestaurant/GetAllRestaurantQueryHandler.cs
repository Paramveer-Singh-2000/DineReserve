using AutoMapper;
using DineReserve.Application.Common;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Restaurants.Queries.GetAllRestaurant
{
    public class GetAllRestaurantQueryHandler(ILogger<GetAllRestaurantQueryHandler> logger, IMapper mapper, IRestaurantRepositories restaurantRepositories) : IRequestHandler<GetAllRestaurantQuery, PagedResult<RestaurantDto>>
    {
        public async Task<PagedResult<RestaurantDto>> Handle(GetAllRestaurantQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting all restaurants");
            var (restaurants, totalCount) = (await restaurantRepositories.GetAllMatchingAsync(request.SearchPhrases, request.PageSize, request.PageNumber, request.SortBy, request.SortDirection));
            var restaurantDto = mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
            var result = new PagedResult<RestaurantDto>(restaurantDto, totalCount, request.PageSize, request.PageNumber);
            
            return result;
        }
    }
}
