using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Server.GraphQL.Types;

public class ProductInputType : InputObjectType<ProductDto>
{
    protected override void Configure(IInputObjectTypeDescriptor<ProductDto> descriptor)
    {
        descriptor.Field(p => p.Id).Ignore();
        descriptor.Field(p => p.Reference).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Name).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Price).Type<NonNullType<DecimalType>>();
    }
}