using DineReserve.Application.Users;
using DineReserve.Domain.Constants;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DineReserve.Application.Tests
{
    public class UserContextTests
    {
        [Fact]
        public void GetCurrentUser_AuthenticatedUser_ReturnsExpectedUser()
        {
            // Arrange
            var dob = new DateOnly(1990, 1, 1);
            var httpcontext = new Mock<IHttpContextAccessor>();
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.Email, "john.doe@example.com"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("Nationality", "American"),
                new Claim("DateOfBirth", dob.ToString("yyyy-MM-dd"))
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claim,"test"));
            httpcontext.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var userContext = new UserContext(httpcontext.Object);

            // Act

            var currentUser = userContext.GetCurrentUser();

            // Assert
            currentUser.Should().NotBeNull();
            currentUser!.UserId.Should().Be("123");
            currentUser.Email.Should().Be("john.doe@example.com");
            currentUser.Roles.Should().ContainInOrder(UserRoles.Admin, UserRoles.User);
            currentUser.Nationality.Should().Be("American");
            currentUser.DateOfBirth.Should().Be(dob);




        }

        //[Fact]
        //public void GetCurrentUser_WithNoUsercontextPresent_ThrowsException()
        //{
        //    // Arrange
        //    var httpcontext = new Mock<IHttpContextAccessor>();


        //}
}
}
