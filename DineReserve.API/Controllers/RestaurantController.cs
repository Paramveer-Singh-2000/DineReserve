using DineReserve.Application.Restaurants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DineReserve.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController(IRestaurantService restaurantService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetRestaurants()
        {
            var restaurants = await restaurantService.GetAllRestaurants();
            return Ok(restaurants );
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetRestaurantsById([FromRoute]int Id)
        {
            var restaurant = await restaurantService.GetRestaurantById(Id);
            if(restaurant is null )
            {
                return NotFound();
            }
            return Ok(restaurant);
        }
    }
}
