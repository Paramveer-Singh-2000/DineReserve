using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Users.Commands.AssignUserRole
{
    public class AssignUserRoleCommand : IRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
