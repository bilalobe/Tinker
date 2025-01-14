using Microsoft.Extensions.Logging;

namespace Tinker.Infrastructure.Integration.Http.Handlers;

public class ErrorHandler : DelegatingHandler
{
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(ILogger<ErrorHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending request to {Url}", request.RequestUri);
            throw;
        }
    }
}