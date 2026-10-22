using DineReserve.Application.Users;
using DineReserve.Application.Users.Commands.UpdateUserDetail;
using DineReserve.Domain.Entities;
using DineReserve.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DineReserve.Application.Tests.Users;

public class UpdateUserDetailCommandHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IUserStore<User> _userStore = Substitute.For<IUserStore<User>>();
    private readonly UpdateUserDetailCommandHandler _sut;

    public UpdateUserDetailCommandHandlerTests()
    {
        _sut = new UpdateUserDetailCommandHandler(Substitute.For<ILogger<UpdateUserDetailCommandHandler>>(), _userContext, _userStore);
    }

    private void ActingAs(string userId) =>
        _userContext.GetCurrentUser().Returns(new CurrentUser(userId, "user@test.com", [], null, null));

    [Fact]
    public async Task Handle_UpdatesDateOfBirthAndNationality_ForCurrentUser()
    {
        ActingAs("user-1");
        var dbUser = new User { Id = "user-1" };
        _userStore.FindByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(dbUser);

        var command = new UpdateUserDetailCommand { DateOfBirth = new DateOnly(2000, 1, 1), Nationality = "USA" };
        await _sut.Handle(command, CancellationToken.None);

        dbUser.DateOfBirth.Should().Be(new DateOnly(2000, 1, 1));
        dbUser.Nationality.Should().Be("USA");
        await _userStore.Received(1).UpdateAsync(dbUser, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenCurrentUserHasNoMatchingDbRecord()
    {
        ActingAs("ghost-user");
        _userStore.FindByIdAsync("ghost-user", Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = () => _sut.Handle(new UpdateUserDetailCommand { Nationality = "India" }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await _userStore.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
