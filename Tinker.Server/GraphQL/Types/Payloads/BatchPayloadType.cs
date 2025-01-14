namespace Tinker.Server.GraphQL.Types.Payloads;

public class BatchPayloadType : ObjectType<BatchPayload>
{
    protected override void Configure(IObjectTypeDescriptor<BatchPayload> descriptor)
    {
        descriptor
            .Field(b => b.Batch)
            .Type<NonNullType<BatchType>>();

        descriptor
            .Field(b => b.Success)
            .Type<NonNullType<BooleanType>>();
    }
}