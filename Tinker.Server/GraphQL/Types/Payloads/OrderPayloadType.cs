namespace Tinker.Server.GraphQL.Types.Payloads;

public class OrderPayloadType : ObjectType<OrderPayload>
{
    protected override void Configure(IObjectTypeDescriptor<OrderPayload> descriptor)
    {
        descriptor
            .Field(o => o.Order)
            .Type<NonNullType<OrderType>>();

        descriptor
            .Field(o => o.Success)
            .Type<NonNullType<BooleanType>>();
    }
}