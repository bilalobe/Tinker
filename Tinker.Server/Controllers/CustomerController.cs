using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinker.Shared.DTOs.Customers;
using CustomerStatistics = Tinker.Server.Services.Domain.Customers.CustomerStatistics;

namespace Tinker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Customer Management")]
/// <summary>
/// Manages customer information and related operations
/// </summary>
/// <remarks>
/// Requires authenticated access with customer management privileges
/// </remarks>
public class CustomerController : ControllerBase
{
    /// <summary>
    ///     Retrieves all customers
    /// </summary>
    /// <returns>List of customers</returns>
    /// <response code="200">Returns the list of customers</response>
    /// <response code="401">Unauthorized access</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers();

    /// <summary>
    ///     Retrieves a customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer information</returns>
    /// <response code="200">Returns the customer information</response>
    /// <response code="404">Customer not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id);

    /// <summary>
    ///     Creates a new customer record
    /// </summary>
    /// <param name="customer">Customer details</param>
    /// <returns>Created customer information</returns>
    /// <response code="201">Customer created successfully</response>
    /// <response code="400">Invalid customer data</response>
    /// <remarks>
    ///     Sample request:
    ///     POST /api/customers
    ///     {
    ///     "name": "John Doe",
    ///     "email": "john@example.com",
    ///     "phone": "123-456-7890"
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerDto customer);

    /// <summary>
    ///     Searches for customers by term
    /// </summary>
    /// <param name="term">Search term</param>
    /// <returns>List of customers matching the search term</returns>
    /// <response code="200">Returns the list of customers</response>
    /// <response code="400">Invalid search term</response>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> SearchCustomers([FromQuery] string term);

    /// <summary>
    ///     Retrieves statistics for a customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer statistics</returns>
    /// <response code="200">Returns the customer statistics</response>
    /// <response code="404">Customer not found</response>
    [HttpGet("{id}/statistics")]
    public async Task<ActionResult<CustomerStatistics>> GetStatistics(int id);
}