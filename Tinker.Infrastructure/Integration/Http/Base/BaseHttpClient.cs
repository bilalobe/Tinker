using System.Net.Http.Json;
using System.Text.Json;

namespace Tinker.Infrastructure.Integration.Http.Base;

public abstract class BaseHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpResiliencePipeline _pipeline;
    private readonly ILogger _logger;

    protected BaseHttpClient(
        HttpClient httpClient,
        IHttpResiliencePipeline pipeline,
        ILogger logger)
    {
        _httpClient = httpClient;
        _pipeline = pipeline;
        _logger = logger;
    }

    protected async Task<T?> GetAsync<T>(string endpoint) 
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(endpoint, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling {Endpoint}", endpoint);
                throw;
            }
        });
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data, (JsonSerializerOptions?)null, ct);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting to {Endpoint}", endpoint);
                throw;
            }
        });
    }
}