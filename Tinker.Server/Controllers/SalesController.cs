using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinker.Shared.DTOs.Extras;
using Tinker.Shared.DTOs.Orders;
using Tinker.Shared.DTOs.Payments;

namespace Tinker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Sales Management")]
/// <summary>
/// Manages sales operations including order processing and payment handling
/// </summary>
/// <remarks>
/// Requires authenticated access with valid JWT token
/// </remarks>
public class SalesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    ///     Process a new sale transaction
    /// </summary>
    /// <param name="order">The order details</param>
    /// <returns>The result of the sale processing</returns>
    /// <response code="200">Sale processed successfully</response>
    /// <response code="400">Invalid order data</response>
    /// <response code="401">Unauthorized access</response>
    /// <remarks>
    ///     Sample request:
    ///     POST /api/sales/process
    ///     {
    ///     "customerId": 1,
    ///     "items": [
    ///     {
    ///     "productId": 1,
    ///     "quantity": 2
    ///     }
    ///     ]
    ///     }
    /// </remarks>
    [HttpPost("process")]
    [ProducesResponseType(typeof(OrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> ProcessSale([FromBody] OrderDto orderDto)
    {
        var command = new ProcessSaleCommand { Order = orderDto };
        var result = await mediator.Send(command);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Error);
    }

    /// <summary>
    ///     Process a payment for a sale
    /// </summary>
    /// <response code="200">Payment processed successfully</response>
    /// <response code="400">Invalid payment data</response>
    /// <response code="404">Order not found</response>
    [HttpPost("payment")]
    [ProducesResponseType(typeof(PaymentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResult>> ProcessPayment(PaymentDto payment);

    /// <summary>
    ///     Retrieves sales history within a date range
    /// </summary>
    /// <response code="200">Returns the sales history</response>
    /// <response code="400">Invalid date range</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetHistory([FromQuery] DateRange range);
}