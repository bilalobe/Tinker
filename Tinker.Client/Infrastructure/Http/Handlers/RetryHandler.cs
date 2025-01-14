namespace Tinker.Client.Infrastructure.Http.Handlers;

public class RetryHandler(ILogger<RetryHandler> logger) : DelegatingHandler
{
    private const int MaxRetries = 3;

    private static readonly TimeSpan[] RetryDelays =
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(3),
        TimeSpan.FromSeconds(5)
    };

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken  cancellationToken)
    {
        for (var i = 0; i <= MaxRetries; i++)
            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode || i == MaxRetries)
                    return response;

                logger.LogWarning("Request failed with {StatusCode}. Attempt {Attempt} of {MaxRetries}",
                    response.StatusCode, i + 1, MaxRetries);

                await Task.Delay(RetryDelays[i], cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                if (i == MaxRetries)
                    throw;

                logger.LogWarning(ex, "Request failed. Attempt {Attempt} of {MaxRetries}",
                    i + 1, MaxRetries);

                await Task.Delay(RetryDelays[i], cancellationToken);
            }

        throw new HttpRequestException("Max retries exceeded");
    }
}