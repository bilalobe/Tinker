using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinker.Infrastructure.Identity.Core.Interfaces;
using Tinker.Shared.Models.Auth;
using Tinker.Shared.Models.Responses;

namespace Tinker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Authentication")]
/// <summary>
/// Handles user authentication and token management
/// </summary>
/// <remarks>
/// Public endpoints that don't require authentication
/// </remarks>
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    /// <summary>
    ///     Authenticates a user and returns an access token
    /// </summary>
    /// <param name="input">Login credentials</param>
    /// <returns>Authentication token response</returns>
    /// <response code="200">Successfully authenticated</response>
    /// <response code="400">Invalid credentials</response>
    /// <remarks>
    ///     Sample request:
    ///     POST /api/auth/login
    ///     {
    ///     "username": "user@example.com",
    ///     "password": "password123"
    ///     }
    ///     Sample response:
    ///     {
    ///     "token": "eyJhbGci...",
    ///     "expiresAt": "2024-01-01T00:00:00Z"
    ///     }
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Login(LoginInput input)
    {
        return Ok(await _identityService.Login(input));
    }

    /// <summary>
    ///     Refreshes an expired access token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <returns>New access token</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="400">Invalid refresh token</response>
    /// <response code="401">Refresh token expired</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Refresh(string token)
    {
        return Ok(await _identityService.RefreshToken(token));
    }
}