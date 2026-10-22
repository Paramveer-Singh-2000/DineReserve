using DineReserve.Application.Users.Commands.AssignUserRole;
using DineReserve.Application.Users.Commands.UnAssignUserRole;
using DineReserve.Application.Users.Commands.UpdateUserDetail;
using DineReserve.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DineReserve.API.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public class IdentityController(IMediator mediator) : ControllerBase
    {
        [HttpPatch("user")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetails(UpdateUserDetailCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> AssignUserRole(AssignUserRoleCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }
        [HttpDelete]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UnAssignUserRole(UnAssignUserRoleCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }
    }
}
