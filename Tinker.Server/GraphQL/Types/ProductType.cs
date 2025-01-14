using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Server.GraphQL.Types;

public class ProductType : ObjectType<ProductDto>
{
    protected override void Configure(IObjectTypeDescriptor<ProductDto> descriptor)
    {
        descriptor
            .Description("Represents a product in the inventory");

        descriptor
            .Field(p => p.Id)
            .Type<NonNullType<IdType>>()
            .Description("Unique identifier of the product");

        descriptor
            .Field(p => p.Reference)
            .Type<NonNullType<StringType>>()
            .Description("Product reference code");

        descriptor
            .Field(p => p.Name)
            .Type<NonNullType<StringType>>()
            .Description("Product name");

        descriptor
            .Field(p => p.Price)
            .Type<NonNullType<DecimalType>>()
            .Description("Product price");

        descriptor
            .Field(p => p.Quantity)
            .Type<IntType>()
            .Description("Current stock quantity");

        descriptor
            .Field(p => p.MinimumStockLevel)
            .Type<IntType>()
            .Description("Minimum stock threshold");


        descriptor
            .Field(p => p.Description)
            .Type<StringType>()
            .Description("Product description");

        descriptor
            .Field(p => p.BatchNumber)
            .Type<StringType>()
            .Description("Batch identification number");

        descriptor
            .Field(p => p.ExpiryDate)
            .Type<DateTimeType>()
            .Description("Product expiry date");

        descriptor
            .Field(p => p.RequiresRx)
            .Type<BooleanType>()
            .Description("Indicates if prescription is required");

        descriptor
            .Field(p => p.StorageConditions)
            .Type<StringType>()
            .Description("Storage requirements");
    }
}