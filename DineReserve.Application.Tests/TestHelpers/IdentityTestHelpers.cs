using DineReserve.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace DineReserve.Application.Tests.TestHelpers;

/// <summary>
/// UserManager&lt;T&gt; and RoleManager&lt;T&gt; have no interfaces, but their members are
/// virtual, so NSubstitute can still create working mocks as long as we satisfy the
/// constructors (most dependencies can be null since we never call the real base logic
/// that would touch them - only the virtual methods we explicitly stub get invoked).
/// </summary>
public static class IdentityTestHelpers
{
    public static UserManager<User> MockUserManager(IUserStore<User>? store = null)
    {
        store ??= Substitute.For<IUserStore<User>>();
        return Substitute.For<UserManager<User>>(store, null, null, null, null, null, null, null, null);
    }

    public static RoleManager<IdentityRole> MockRoleManager(IRoleStore<IdentityRole>? store = null)
    {
        store ??= Substitute.For<IRoleStore<IdentityRole>>();
        return Substitute.For<RoleManager<IdentityRole>>(store, null, null, null, null);
    }
}
