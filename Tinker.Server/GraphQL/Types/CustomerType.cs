using Tinker.Shared.DTOs.Customers;

namespace Tinker.Server.GraphQL.Types;

public class CustomerType : ObjectType<CustomerDto>
{
    protected override void Configure(IObjectTypeDescriptor<CustomerDto> descriptor)
    {
        descriptor
            .Field(c => c.Id)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(c => c.Name)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.Email)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.PhoneNumber)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.LoyaltyPoints)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(c => c.MembershipTier)
            .Type<NonNullType<StringType>>();
    }
}