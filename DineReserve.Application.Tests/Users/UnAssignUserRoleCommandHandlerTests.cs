using DineReserve.Application.Tests.TestHelpers;
using DineReserve.Application.Users.Commands.UnAssignUserRole;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Users;

public class UnAssignUserRoleCommandHandlerTests
{
    private readonly UserManager<User> _userManager = IdentityTestHelpers.MockUserManager();
    private readonly RoleManager<IdentityRole> _roleManager = IdentityTestHelpers.MockRoleManager();
    private readonly UnAssignUserRoleCommandHandler _sut;

    public UnAssignUserRoleCommandHandlerTests()
    {
        _sut = new UnAssignUserRoleCommandHandler(Substitute.For<ILogger<UnAssignUserRoleCommandHandler>>(), _userManager, _roleManager);
    }

    [Fact]
    public async Task Handle_RemovesUserFromRole_WhenUserAndRoleExist()
    {
        var user = new User { Email = "someone@test.com" };
        var role = new IdentityRole("Owner");
        _userManager.FindByEmailAsync("someone@test.com").Returns(user);
        _roleManager.FindByNameAsync("Owner").Returns(role);

        await _sut.Handle(new UnAssignUserRoleCommand { UserEmail = "someone@test.com", RoleName = "Owner" }, CancellationToken.None);

        await _userManager.Received(1).RemoveFromRoleAsync(user, "Owner");
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenUserDoesNotExist()
    {
        _userManager.FindByEmailAsync("missing@test.com").Returns((User?)null);

        var act = () => _sut.Handle(new UnAssignUserRoleCommand { UserEmail = "missing@test.com", RoleName = "Owner" }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await _userManager.DidNotReceive().RemoveFromRoleAsync(Arg.Any<User>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRoleDoesNotExist()
    {
        var user = new User { Email = "someone@test.com" };
        _userManager.FindByEmailAsync("someone@test.com").Returns(user);
        _roleManager.FindByNameAsync("NotARole").Returns((IdentityRole?)null);

        var act = () => _sut.Handle(new UnAssignUserRoleCommand { UserEmail = "someone@test.com", RoleName = "NotARole" }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
