using HotChocolate;
using HotChocolate.Types;
using Tinker.Infrastructure.Identity.Core.Interfaces;
using Tinker.Shared.Exceptions;
using Tinker.Shared.Models.Auth;

namespace Tinker.Infrastructure.Identity.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class UserMutations
{
    [Error(typeof(ValidationException))]
    public async Task<UserPayload> CreateUser(
        [Service] IIdentityService identityService,
        CreateUserInput            input)
    {
        var user = await identityService.CreateUser(input);
        return new UserPayload(user);
    }

    [Error(typeof(ValidationException))]
    public async Task<TokenPayload> Login(
        [Service] IIdentityService identityService,
        LoginInput                 input)
    {
        var token = await identityService.Login(input);
        return new TokenPayload(token);
    }
}