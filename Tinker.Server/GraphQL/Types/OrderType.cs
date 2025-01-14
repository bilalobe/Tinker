using Tinker.Shared.DTOs.Orders;

namespace Tinker.Server.GraphQL.Types;

public class OrderType : ObjectType<OrderDto>
{
    protected override void Configure(IObjectTypeDescriptor<OrderDto> descriptor)
    {
        descriptor
            .Field(o => o.Id)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(o => o.CustomerId)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(o => o.OrderDate)
            .Type<NonNullType<DateTimeType>>();

        descriptor
            .Field(o => o.TotalAmount)
            .Type<NonNullType<DecimalType>>();
    }
}