using Tinker.Shared.Models.Responses;

namespace Tinker.Server.GraphQL.Types;

public class TokenType : ObjectType<TokenResponse>
{
    protected override void Configure(IObjectTypeDescriptor<TokenResponse> descriptor)
    {
        descriptor.Field(t => t.Token).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.RefreshToken).Type<NonNullType<StringType>>();
    }
}