using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Inventory Management")]
/// <summary>
/// Manages inventory operations including stock updates and monitoring
/// </summary>
/// <remarks>
/// Requires authenticated access with inventory management privileges
/// </remarks>
public class InventoryController(
    IProductService              productService,
    ILogger<InventoryController> logger,
    IMetricsService              metricsService)
    : ControllerBase
{
    private readonly ILogger<InventoryController> _logger = logger;
    private readonly IProductService _productService = productService;

    /// <summary>
    ///     Updates the stock quantity for a specific product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Stock update details</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Stock updated successfully</response>
    /// <response code="400">Invalid update request</response>
    /// <response code="404">Product not found</response>
    /// <remarks>
    ///     Sample request:
    ///     PUT /api/inventory/1/stock
    ///     {
    ///     "quantity": 50,
    ///     "operation": "Add"
    ///     }
    /// </remarks>
    [HttpPut("{id}/stock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateRequest request)
    {
        metricsService.StartTimer("stock.update");
        try
        {
            await _productService.UpdateStock(id, request.Quantity, request.Operation);
            metricsService.IncrementCounter("stock.updates");
            return NoContent();
        }
        finally
        {
            metricsService.StopTimer("stock.update");
        }
    }

    /// <summary>
    ///     Gets all products with low stock
    /// </summary>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts([FromQuery] int threshold = 10)
    {
        var products = await _productService.GetLowStockProducts(threshold);
        return Ok(products);
    }
}