using DineReserve.Application.Commands.CreateRestaurant;
using DineReserve.Application.Restaurants;
using DineReserve.Application.Restaurants.Dtos;
using DineReserve.Application.Restaurants.Queries.GetAllRestaurant;
using DineReserve.Application.Restaurants.Queries.GetRestaurantById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DineReserve.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetRestaurants()
        {
            var restaurants = await mediator.Send(new GetAllRestaurantQuery());
            return Ok(restaurants );
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetRestaurantsById([FromRoute]int Id)
        {
            var restaurant = await mediator.Send(new GetRestaurantByIdQuery()
            {
                Id = Id
            });
            if(restaurant is null )
            {
                return NotFound();
            }
            return Ok(restaurant);
        }

        [HttpPost]
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
