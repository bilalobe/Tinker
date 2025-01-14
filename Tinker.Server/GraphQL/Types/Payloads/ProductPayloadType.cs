namespace Tinker.Server.GraphQL.Types.Payloads;

public class ProductPayloadType : ObjectType<ProductPayload>
{
    protected override void Configure(IObjectTypeDescriptor<ProductPayload> descriptor)
    {
        descriptor
            .Field(p => p.Product)
            .Type<NonNullType<ProductType>>();

        descriptor
            .Field(p => p.Success)
            .Type<NonNullType<BooleanType>>();
    }
}