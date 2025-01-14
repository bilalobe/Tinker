using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tinker.Server.Controllers;

/// <summary>
///     Handles compliance-related operations and monitoring
/// </summary>
/// <remarks>
///     Requires authenticated access with compliance management privileges
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Compliance Management")]
public class ComplianceController : ControllerBase
{
    /// <summary>
    ///     Retrieves all compliance logs
    /// </summary>
    /// <returns>List of compliance logs</returns>
    /// <response code="200">Returns the list of compliance logs</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Forbidden - Admin roles required</response>
    /// <remarks>
    ///     Sample request:
    ///     GET /api/compliance/logs
    /// </remarks>
    [HttpGet("logs")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<ComplianceLog>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<ComplianceLog>>> GetComplianceLogs();

    /// <summary>
    ///     Creates a new compliance log entry
    /// </summary>
    /// <param name="log">Compliance log details</param>
    /// <returns>The created compliance log</returns>
    /// <response code="201">Compliance log created successfully</response>
    /// <response code="400">Invalid log data</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Forbidden - Admin roles required</response>
    /// <remarks>
    ///     Sample request:
    ///     POST /api/compliance/logs
    ///     {
    ///     "message": "Compliance check passed",
    ///     "timestamp": "2024-01-01T12:00:00Z"
    ///     }
    /// </remarks>
    [HttpPost("logs")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ComplianceLog), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ComplianceLog>> CreateComplianceLog(ComplianceLog log);
}