using Tinker.Core.Domain.Products.Repositories;
using Tinker.Core.Services.Products.Interfaces;
using Tinker.Shared.DTOs.Inventory;
using Tinker.Shared.Exceptions;

namespace Tinker.Core.Services.Products;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<IEnumerable<ProductDto>> GetProducts()
    {
        var products = await productRepository.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Reference = p.Reference,
            CategoryName = p.CategoryName,
            Price = p.Price,
            Quantity = p.Quantity,
            Description = p.Description,
            BatchItems = p.BatchItems.Select(b => new BatchDto
            {
                BatchNumber = b.BatchNumber,
                ExpiryDate = b.ExpiryDate,
                Quantity = b.Quantity
            }).ToList()
        });
    }

    public async Task<ProductDto?> GetProductById(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Reference = product.Reference,
            CategoryName = product.CategoryName,
            Price = product.Price,
            Quantity = product.Quantity,
            Description = product.Description,
            BatchItems = product.BatchItems.Select(b => new BatchDto
            {
                BatchNumber = b.BatchNumber,
                ExpiryDate = b.ExpiryDate,
                Quantity = b.Quantity
            }).ToList()
        };
    }

    public async Task AddProduct(ProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Reference = productDto.Reference,
            CategoryName = productDto.CategoryName,
            Price = productDto.Price,
            Quantity = productDto.Quantity,
            Description = productDto.Description
        };

        await productRepository.AddAsync(product);
    }

    public async Task UpdateProduct(ProductDto productDto)
    {
        var product = await productRepository.GetByIdAsync(productDto.Id);
        if (product == null) throw new NotFoundException($"Product {productDto.Id} not found");

        product.Name = productDto.Name;
        product.Reference = productDto.Reference;
        product.CategoryName = productDto.CategoryName;
        product.Price = productDto.Price;
        product.Quantity = productDto.Quantity;
        product.Description = productDto.Description;

        await productRepository.UpdateAsync(product);
    }

    public async Task UpdateStock(int productId, int quantity, string operation)
    {
        var product = await productRepository.GetByIdAsync(productId)
                      ?? throw new NotFoundException($"Product {productId} not found");

        switch (operation.ToLower())
        {
            case "add":
                product.Quantity += quantity;
                break;
            case "remove":
                if (product.Quantity < quantity)
                    throw new ArgumentException($"Insufficient stock for product {product.Reference}");
                product.Quantity -= quantity;
                break;
            default:
                throw new ArgumentException($"Invalid operation: {operation}");
        }

        await productRepository.UpdateAsync(product);
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProducts(int threshold = 10)
    {
        var products = await productRepository.GetLowStockProductsAsync(threshold);
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Reference = p.Reference,
            CategoryName = p.CategoryName,
            Price = p.Price,
            Quantity = p.Quantity,
            Description = p.Description
        });
    }

    public async Task<IEnumerable<ProductDto>> GetExpiringProducts(int daysThreshold)
    {
        var products = await productRepository.GetExpiringProductsAsync(daysThreshold);
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Reference = p.Reference,
            CategoryName = p.CategoryName,
            Price = p.Price,
            Quantity = p.Quantity,
            Description = p.Description
        });
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByBatch(string batchNumber)
    {
        var products = await productRepository.GetAllAsync();
        return products.Where(p => p.BatchItems.Any(b => b.BatchNumber == batchNumber))
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Reference = p.Reference,
                CategoryName = p.CategoryName,
                Price = p.Price,
                Quantity = p.Quantity,
                Description = p.Description
            });
    }

    public async Task<IEnumerable<ProductDto>> GetRxProducts()
    {
        var products = await productRepository.GetAllAsync();
        return products.Where(p => p.RequiresRx)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Reference = p.Reference,
                CategoryName = p.CategoryName,
                Price = p.Price,
                Quantity = p.Quantity,
                Description = p.Description
            });
    }

    public async Task UpdateBatch(int productId, string batchNumber, DateTime expiryDate)
    {
        var product = await productRepository.GetByIdAsync(productId)
                      ?? throw new NotFoundException($"Product {productId} not found");

        var batch = product.BatchItems.FirstOrDefault(b => b.BatchNumber == batchNumber);
        if (batch == null)
        {
            batch = new Batch
            {
                BatchNumber = batchNumber,
                ExpiryDate = expiryDate,
                Quantity = 0
            };
            product.BatchItems.Add(batch);
        }
        else
        {
            batch.ExpiryDate = expiryDate;
        }

        await productRepository.UpdateAsync(product);
    }

    public async Task UpdateRxStatus(int productId, bool requiresRx)
    {
        var product = await productRepository.GetByIdAsync(productId)
                      ?? throw new NotFoundException($"Product {productId} not found");

        product.RequiresRx = requiresRx;
        await productRepository.UpdateAsync(product);
    }

    public async Task<Product?> GetProductEntityById(int id)
    {
        return await productRepository.GetByIdAsync(id);
    }

    public async Task UpdateProductEntity(Product product)
    {
        await productRepository.UpdateAsync(product);
    }
}