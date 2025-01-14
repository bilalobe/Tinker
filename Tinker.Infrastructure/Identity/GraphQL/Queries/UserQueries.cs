using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;
using Tinker.Infrastructure.Identity.Core.Interfaces;
using Tinker.Infrastructure.Identity.Core.Models;

namespace Tinker.Infrastructure.Identity.GraphQL.Queries;

[ExtendObjectType("Query")]
public class UserQueries
{
    [Authorize(Roles = "Admin")]
    public async Task<IEnumerable<ApplicationUser>> GetUsers(
        [Service] IIdentityService identityService)
    {
        return await identityService.GetUsers();
    }

    [Authorize]
    public async Task<ApplicationUser> GetCurrentUser(
        [Service]     IIdentityService identityService,
        [GlobalState] string           userId)
    {
        return await identityService.GetUserById(userId);
    }
}