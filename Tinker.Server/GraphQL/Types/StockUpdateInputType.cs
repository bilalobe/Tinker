namespace Tinker.Server.GraphQL.Types;

public class StockUpdateInputType : InputObjectType
{
    protected override void Configure(IInputObjectTypeDescriptor descriptor)
    {
        descriptor
            .Field("quantity")
            .Type<NonNullType<IntType>>();

        descriptor
            .Field("operation")
            .Type<NonNullType<StringType>>()
            .Description("Operation type: 'add' or 'remove'");
    }
}