using Tinker.Infrastructure.Identity.Core.Models;

namespace Tinker.Server.GraphQL.Types;

public class UserType : ObjectType<ApplicationUser>
{
    protected override void Configure(IObjectTypeDescriptor<ApplicationUser> descriptor)
    {
        descriptor.Field(u => u.Id).Type<NonNullType<IdType>>();
        descriptor.Field(u => u.UserName).Type<NonNullType<StringType>>();
        descriptor.Field(u => u.Email).Type<NonNullType<StringType>>();
        descriptor.Field(u => u.PhoneNumber).Type<StringType>();
        descriptor.Field(u => u.Roles).Type<ListType<RoleType>>();
        descriptor.Field(u => u.FirstName).Type<StringType>();
        descriptor.Field(u => u.LastName).Type<StringType>();
        descriptor.Field(u => u.Profile).Type<UserProfileType>();
    }
}