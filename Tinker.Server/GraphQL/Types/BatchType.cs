namespace Tinker.Server.GraphQL.Types;

public class BatchType : ObjectType<BatchResponse>
{
    protected override void Configure(IObjectTypeDescriptor<BatchResponse> descriptor)
    {
        descriptor
            .Field(b => b.BatchNumber)
            .Type<NonNullType<StringType>>()
            .Description("Batch identification number");

        descriptor
            .Field(b => b.ExpiryDate)
            .Type<NonNullType<DateTimeType>>()
            .Description("Expiry date for this batch");

        descriptor
            .Field(b => b.Quantity)
            .Type<NonNullType<IntType>>()
            .Description("Current quantity in this batch");
    }
}