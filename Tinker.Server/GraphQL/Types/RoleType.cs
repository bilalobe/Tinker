using Microsoft.AspNetCore.Identity;

namespace Tinker.Server.GraphQL.Types;

public class RoleType : ObjectType<IdentityRole>
{
    protected override void Configure(IObjectTypeDescriptor<IdentityRole> descriptor)
    {
        descriptor.Field(r => r.Id).Type<NonNullType<IdType>>();
        descriptor.Field(r => r.Name).Type<NonNullType<StringType>>();
    }
}