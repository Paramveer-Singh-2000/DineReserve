using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Infrastructure.Authorization
{
    public class PolicyNames
    {
        public const string HasNationality = "HasNationality";
        public const string AtLeast20 = "AtLeast20";
        public const string CreatedAtLeast2Restaurant = "CreatedAtLeast2Restaurant";
    }
    public class  AppClaimsTypes
    {
        public const string Nationality = "Nationality";
        public const string DateOfBirth = "DateOfBirth";
    }
}
