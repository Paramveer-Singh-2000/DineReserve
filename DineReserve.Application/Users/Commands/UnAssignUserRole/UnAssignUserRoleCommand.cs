using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Users.Commands.UnAssignUserRole
{
    public class UnAssignUserRoleCommand :IRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
