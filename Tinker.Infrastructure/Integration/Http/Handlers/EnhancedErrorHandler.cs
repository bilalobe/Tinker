using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Infrastructure.Monitoring.Core.Models;
using Tinker.Shared.Exceptions.Common;

namespace Tinker.Infrastructure.Integration.Http.Handlers;
public class EnhancedErrorHandler : DelegatingHandler
{
    private readonly IToastService _toastService;
    private readonly ILogger<EnhancedErrorHandler> _logger;
    private readonly IMetricsService _metrics;

    public EnhancedErrorHandler(
        IToastService toastService,
        ILogger<EnhancedErrorHandler> logger,
        IMetricsService metrics)
    {
        _toastService = toastService;
        _logger = logger;
        _metrics = metrics;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
                _metrics.IncrementCounter("http.errors", new[]
                {
                    new MetricDimension("status_code", response.StatusCode.ToString()),
                    new MetricDimension("endpoint", request.RequestUri?.PathAndQuery ?? "unknown")
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            await HandleException(ex, request);
            throw;
        }
    }

    private async Task HandleErrorResponse(HttpResponseMessage response)
    {
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        var message = GetUserFriendlyMessage(response.StatusCode, error);
        _toastService.ShowError(message);
        
        _logger.LogError("HTTP {StatusCode}: {Message}", 
            response.StatusCode, error?.Message ?? "Unknown error");
    }

    private Task HandleException(Exception ex, HttpRequestMessage request)
    {
        var (message, severity) = ex switch
        {
            HttpRequestException _ => ("Network error. Please check your connection.", LogLevel.Error),
            TaskCanceledException _ => ("Request timeout. Please try again.", LogLevel.Warning),
            _ => ("An unexpected error occurred.", LogLevel.Critical)
        };

        _toastService.ShowError(message);
        _logger.Log(severity, ex, "Request failed: {Method} {Url}", 
            request.Method, request.RequestUri);

        return Task.CompletedTask;
    }
}