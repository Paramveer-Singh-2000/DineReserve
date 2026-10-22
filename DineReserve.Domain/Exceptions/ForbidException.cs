using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Domain.Exceptions
{
    public class ForbidException(string message = "You are not allowed to perform this operation.") : Exception(message)
    {
    }
}
