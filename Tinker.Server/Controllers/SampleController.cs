using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinker.Infrastructure.Core.Caching.Interfaces;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Server.Controllers.Base;

namespace Tinker.Server.Controllers;

[ApiVersion("1.0")]
[Tags("Sample Operations")]
[Authorize]
public class SampleController(
    ILogger<SampleController> logger,
    IMetricsService           metrics,
    ICacheService             cache)
    : ApiController(logger, metrics, cache)
{
    /// <summary>
    ///     Retrieves a sample resource
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "CustomPolicy")]
    [ProducesResponseType(typeof(SampleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
    [ResponseCache(Duration = 60)]
    public Task<ActionResult<SampleResponse>> Get()
    {
        return ExecuteWithMetrics(
            async () => new SampleResponse
            {
                Message = "Authorized",
                Timestamp = DateTime.UtcNow
            },
            "sample.get",
            new CacheOptions
            {
                Expiry = TimeSpan.FromMinutes(1),
                Tags = new HashSet<string> { "sample" }
            });
    }
}