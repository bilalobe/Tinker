using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinker.Infrastructure.Core.Caching.Interfaces;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Server.Controllers.Base;
using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[Tags("Product Management")]
[Authorize]
/// <summary>
/// Manages product catalog operations including CRUD operations
/// </summary>
/// <remarks>
/// Requires authenticated access. Some operations may require additional privileges.
/// </remarks>
public class ProductController(
    IProductService            productService,
    ILogger<ProductController> logger,
    IMetricsService            metrics,
    ICacheService              cache)
    : ApiController(logger, metrics, cache)
{
    private readonly IProductService _productService = productService;

    /// <summary>
    ///     Gets all products
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300)] // 5 minutes cache
    public Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        return ExecuteWithMetrics(
            _productService.GetProducts,
            "products.list",
            new CacheOptions
            {
                Expiry = TimeSpan.FromMinutes(5),
                Tags = new HashSet<string> { "products" }
            });
    }

    /// <summary>
    ///     Gets a specific product by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    /// <summary>
    ///     Creates a new product in the catalog
    /// </summary>
    /// <param name="product">The product details</param>
    /// <returns>The created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid product data</response>
    /// <response code="401">Unauthorized access</response>
    /// <remarks>
    ///     Sample request:
    ///     POST /api/products
    ///     {
    ///     "name": "Aspirin",
    ///     "description": "Pain reliever",
    ///     "price": 9.99,
    ///     "requiresRx": false
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto product)
    {
        await _productService.AddProduct(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    ///     Updates an existing product
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, ProductDto product)
    {
        if (id != product.Id) return BadRequest();
        await _productService.UpdateProduct(product);
        return NoContent();
    }
}