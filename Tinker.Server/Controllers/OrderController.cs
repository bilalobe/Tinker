using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Order Management")]
/// <summary>
/// Manages order operations including retrieval, creation, and status updates
/// </summary>
/// <remarks>
/// Requires authenticated access with appropriate roles
/// </remarks>
public class OrderController : ControllerBase
{
    /// <summary>
    ///     Retrieves all orders
    /// </summary>
    /// <returns>List of orders</returns>
    /// <response code="200">Returns the list of orders</response>
    /// <response code="401">Unauthorized access</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders();

    /// <summary>
    ///     Creates a new order
    /// </summary>
    /// <param name="order">Order details</param>
    /// <returns>The result of the order creation</returns>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Invalid order data</response>
    /// <response code="401">Unauthorized access</response>
    /// <remarks>
    ///     Sample request:
    ///     POST /api/order
    ///     {
    ///     "customerId": 1,
    ///     "items": [
    ///     {
    ///     "productId": 1,
    ///     "quantity": 2,
    ///     "unitPrice": 9.99,
    ///     "discountPercentage": 5
    ///     }
    ///     ]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderResult>> CreateOrder(OrderDto order);

    /// <summary>
    ///     Updates the status of an existing order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="status">New status for the order</param>
    /// <returns>The result of the status update</returns>
    /// <response code="200">Order status updated successfully</response>
    /// <response code="400">Invalid status value</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Order not found</response>
    /// <remarks>
    ///     Sample request:
    ///     PUT /api/order/1/status
    ///     {
    ///     "status": "Completed"
    ///     }
    /// </remarks>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(OrderStatusResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderStatusResult>> UpdateStatus(int id, OrderStatus status);
}