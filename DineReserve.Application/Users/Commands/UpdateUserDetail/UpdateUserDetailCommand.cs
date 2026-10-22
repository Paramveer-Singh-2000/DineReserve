using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Users.Commands.UpdateUserDetail
{
    public class UpdateUserDetailCommand :IRequest
    {
        public DateOnly? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
    }
}
