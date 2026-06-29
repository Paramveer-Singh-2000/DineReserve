using DineReserve.Application.Commands.DeleteRestaurant;
using DineReserve.Application.Restaurants;
using DineReserve.Application.Restaurants.Commands.CreateRestaurant;
using DineReserve.Application.Restaurants.Commands.DeleteRestaurant;
using DineReserve.Application.Restaurants.Commands.UpdateRestaurant;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Application.Restaurants.Queries.GetAllRestaurant;
using DineReserve.Application.Restaurants.Queries.GetRestaurantById;
using DineReserve.Domain.Constants;
using DineReserve.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Threading.Tasks;

namespace DineReserve.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RestaurantController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRestaurants([FromQuery] GetAllRestaurantQuery query)
        {
            var restaurants = await mediator.Send(query);
            return Ok(restaurants );
        }

        [HttpGet("{Id}")]
        [Authorize(policy: PolicyNames.HasNationality)]
        public async Task<ActionResult<RestaurantDto>> GetRestaurantsById([FromRoute]int Id)
        {
            var restaurant = await mediator.Send(new GetRestaurantByIdQuery()
            {
                Id = Id
            });
            return Ok(restaurant);
        }

        [HttpDelete("{Id}")]
        [ProducesResponseType(statusCode:StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode:StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRestaurant([FromRoute] int Id)
        {
             await mediator.Send(new DeleteRestaurantCommand(Id));           
             return NoContent();
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRestaurant([FromRoute] int id, UpdateRestaurantCommand command)
        {
            command.Id = id;
            await mediator.Send(command);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles =UserRoles.Owner)]
        public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantCommand createRestaurant)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int id = await mediator.Send(createRestaurant);
            return CreatedAtAction(nameof(GetRestaurantsById), new { id }, null);
        }
    }
}
