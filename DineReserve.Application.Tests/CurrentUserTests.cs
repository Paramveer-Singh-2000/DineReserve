using DineReserve.Application.Users;
using DineReserve.Domain.Constants;
using FluentAssertions;

namespace DineReserve.Application.Tests
{
    public class CurrentUserTests
    {
        [Theory]
        [InlineData(UserRoles.Admin)]
        [InlineData(UserRoles.User)]
        public void IsInRole_ReturnsTrue_WhenUserHasRole(string roleName)
        {
            var currentUser = new CurrentUser("user1", "user1@example.com",[UserRoles.Admin,UserRoles.User], null, null);

            var isInRole = currentUser.IsInRole(roleName);

            isInRole.Should().BeTrue();

        }

        [Fact]
        public void IsInRole_ReturnsFalse_WhenUserDoesNotHaveRole()
        {
            var currentUser = new CurrentUser("user1", "user1@example.com", [UserRoles.Admin, UserRoles.User], null, null);

            var isInRole = currentUser.IsInRole(UserRoles.Owner);

            isInRole.Should().BeFalse();

        }

        [Fact]
        public void IsInRole_ReturnsFalse_WithNotMatchingRoleCase()
        {
            var currentUser = new CurrentUser("user1", "user1@example.com", [UserRoles.Admin, UserRoles.User], null, null);

            var isInRole = currentUser.IsInRole(UserRoles.Admin.ToLower());

            isInRole.Should().BeFalse();

        }

    }
}
