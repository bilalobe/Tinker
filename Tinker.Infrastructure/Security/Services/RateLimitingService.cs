using System.Net;
using Microsoft.Extensions.Logging;

namespace Tinker.Infrastructure.Security.Services;

public class RateLimitingHandler : DelegatingHandler
{
    private readonly RateLimitingService _rateLimitingService;
    private readonly ILogger<RateLimitingHandler> _logger;

    public RateLimitingHandler(RateLimitingService rateLimitingService, ILogger<RateLimitingHandler> logger)
    {
        _rateLimitingService = rateLimitingService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var clientIp = GetClientIp(request);
        var endpoint = request.RequestUri?.PathAndQuery ?? "";
        var cacheKey = $"rate_limit:{clientIp}:{endpoint}";

        if (await _rateLimitingService.IsRateLimitedAsync(cacheKey))
        {
            _logger.LogWarning("Rate limit exceeded for IP {ClientIp}", clientIp);
            return new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private string GetClientIp(HttpRequestMessage request)
    {
        if (request.Properties.TryGetValue("ClientIp", out var clientIp)) return clientIp.ToString();

        return "unknown";
    }
}