using Microsoft.Extensions.Logging;

namespace Tinker.Infrastructure.Integration.Http.Handlers;

public class RetryHandler : DelegatingHandler
{
    private readonly ILogger<RetryHandler> _logger;

    public RetryHandler(ILogger<RetryHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var retryCount = 3;
        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Retry {RetryCount} for {Url}", i + 1, request.RequestUri);
                if (i == retryCount - 1) throw;
            }
        }

        throw new InvalidOperationException("Max retry attempts exceeded");
    }
}