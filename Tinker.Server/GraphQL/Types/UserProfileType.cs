using Tinker.Infrastructure.Identity.Core.Models;

namespace Tinker.Server.GraphQL.Types;

public class UserProfileType : ObjectType<UserProfile>
{
    protected override void Configure(IObjectTypeDescriptor<UserProfile> descriptor)
    {
        descriptor.Field(p => p.FirstName).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.LastName).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.DateOfBirth).Type<DateType>();
        descriptor.Field(p => p.Address).Type<StringType>();
        descriptor.Field(p => p.City).Type<StringType>();
        descriptor.Field(p => p.State).Type<StringType>();
        descriptor.Field(p => p.ZipCode).Type<StringType>();
        descriptor.Field(p => p.Country).Type<StringType>();
    }
}