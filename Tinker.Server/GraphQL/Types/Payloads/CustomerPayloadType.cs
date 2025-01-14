namespace Tinker.Server.GraphQL.Types.Payloads;

public class CustomerPayloadType : ObjectType<CustomerPayload>
{
    protected override void Configure(IObjectTypeDescriptor<CustomerPayload> descriptor)
    {
        descriptor
            .Field(c => c.Customer)
            .Type<NonNullType<CustomerType>>();

        descriptor
            .Field(c => c.Success)
            .Type<NonNullType<BooleanType>>();
    }
}