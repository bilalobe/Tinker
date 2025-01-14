using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tinker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Reporting")]
/// <summary>
/// Handles generation of various reports related to inventory, expiry, and compliance
/// </summary>
/// <remarks>
/// Requires authenticated access. Some reports may require specific roles.
/// </remarks>
public class ReportController : ControllerBase
{
    /// <summary>
    ///     Retrieves the inventory report
    /// </summary>
    /// <returns>Inventory report data</returns>
    /// <response code="200">Returns the inventory report</response>
    /// <response code="401">Unauthorized access</response>
    /// <remarks>
    ///     Sample request:
    ///     GET /api/report/inventory
    /// </remarks>
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(Report), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Report>> GetInventoryReport();

    /// <summary>
    ///     Retrieves the expiry report for products nearing expiration
    /// </summary>
    /// <param name="daysThreshold">Number of days to check for upcoming expirations</param>
    /// <returns>List of expiry alerts</returns>
    /// <response code="200">Returns the expiry report</response>
    /// <response code="400">Invalid threshold parameter</response>
    /// <response code="401">Unauthorized access</response>
    /// <remarks>
    ///     Sample request:
    ///     GET /api/report/expiry?daysThreshold=30
    /// </remarks>
    [HttpGet("expiry")]
    [ProducesResponseType(typeof(IEnumerable<ExpiryAlert>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ExpiryAlert>>> GetExpiryReport(int daysThreshold);

    /// <summary>
    ///     Retrieves the compliance report
    /// </summary>
    /// <returns>List of compliance logs</returns>
    /// <response code="200">Returns the compliance report</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Forbidden - Admin roles required</response>
    /// <remarks>
    ///     Sample request:
    ///     GET /api/report/compliance
    /// </remarks>
    [HttpGet("compliance")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<ComplianceLog>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<ComplianceLog>>> GetComplianceReport();
}