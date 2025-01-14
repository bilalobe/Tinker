using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Batch Management")]
/// <summary>
/// Manages batch operations related to products
/// </summary>
/// <remarks>
/// Requires authenticated access with batch management privileges
/// </remarks>
public class BatchController(
    IBatchService            batchService,
    IProductService          productService,
    ILogger<BatchController> logger)
    : ControllerBase
{
    private readonly IBatchService _batchService = batchService;
    private readonly ILogger<BatchController> _logger = logger;
    private readonly IProductService _productService = productService;

    /// <summary>
    ///     Retrieves products associated with a specific batch number
    /// </summary>
    /// <param name="batchNumber">Batch number to filter products</param>
    /// <returns>List of products in the specified batch</returns>
    /// <response code="200">Returns the list of products</response>
    /// <response code="400">Invalid batch number</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">No products found for the given batch number</response>
    /// <remarks>
    ///     Sample request:
    ///     GET /api/batch/BN12345
    /// </remarks>
    [HttpGet("{batchNumber}")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBatch(string batchNumber)
    {
        var products = await _productService.GetProductsByBatch(batchNumber);
        return Ok(products);
    }

    /// <summary>
    ///     Updates batch information for a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="input">Batch update details</param>
    /// <returns>No content on successful update</returns>
    /// <response code="204">Batch updated successfully</response>
    /// <response code="400">Invalid update data</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Product not found</response>
    /// <remarks>
    ///     Sample request:
    ///     PUT /api/batch/1/batch
    ///     {
    ///     "batchNumber": "BN67890",
    ///     "expiryDate": "2025-12-31T00:00:00Z"
    ///     }
    /// </remarks>
    [HttpPut("{productId}/batch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateBatch(int productId, BatchUpdateInput input)
    {
        await _batchService.UpdateExpiryDate(input.BatchNumber, input.ExpiryDate);
        await _productService.UpdateBatch(productId, input.BatchNumber, input.ExpiryDate);
        return NoContent();
    }

    [HttpGet("expiring/{daysThreshold}")]
    public async Task<ActionResult<IEnumerable<BatchExpiryDto>>> GetExpiringBatches(int daysThreshold)
    {
        var batches = await _batchService.GetExpiringBatches(daysThreshold);
        var dtos = batches.Select(b => new BatchExpiryDto
        {
            BatchNumber = b.BatchNumber,
            ExpiryDate = b.ExpiryDate,
            DaysUntilExpiry = (b.ExpiryDate - DateTime.UtcNow).Days
        });
        return Ok(dtos);
    }
}