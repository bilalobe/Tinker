namespace Tinker.Server.GraphQL.Types;

public class OrderInputType : InputObjectType<OrderInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<OrderInput> descriptor)
    {
        descriptor
            .Field(o => o.CustomerId)
            .Type<NonNullType<IdType>>();

        descriptor
            .Field(o => o.Items)
            .Type<NonNullType<ListType<OrderItemInputType>>>();

        descriptor
            .Field(o => o.PaymentMethod)
            .Type<NonNullType<PaymentMethodType>>();
    }
}