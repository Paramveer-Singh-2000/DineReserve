using DineReserve.Application.Dishes.Commands.CreateDish;
using DineReserve.Application.Dishes.Commands.DeleteRestaurant;
using DineReserve.Application.Dishes.Dtos;
using DineReserve.Application.Dishes.Queries.GetAllDishes;
using DineReserve.Application.Dishes.Queries.GetDishById;
using DineReserve.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DineReserve.API.Controllers
{
    [Route("api/restaurants/{restaurantId}/dishes")]
    [ApiController]
    [Authorize]
    public class DishesController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateDish([FromRoute] int restaurantId, CreateDishCommand command)
        {
            command.RestaurantId = restaurantId;
            var dishId = await mediator.Send(command);
            return CreatedAtAction(nameof(GetDishById), new { restaurantId, dishId }, null);
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.AtLeast20)]
        public async Task<ActionResult<IEnumerable<DishDto>>> GetDishes([FromRoute] int restaurantId)
        {
            var dishes = await mediator.Send(new GetAllDishesQuery(restaurantId));
            return Ok(dishes);
        }

        [HttpGet("{dishId}")]
        public async Task<ActionResult<DishDto>> GetDishById([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = await mediator.Send(new GetDishByIdQuery(restaurantId, dishId));
            return Ok(dish);
        }

        [HttpDelete("{dishId}")]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDish([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            await mediator.Send(new DeleteDishCommand(restaurantId, dishId));
            return NoContent();
        }
    }
}