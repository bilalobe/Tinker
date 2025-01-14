using Tinker.Infrastructure.Integration.Http.Handlers;
using Tinker.Shared.Components.ErrorHandling;

namespace Tinker.Infrastructure.Configuration.Setup;

using Microsoft.Extensions.DependencyInjection;

public static class ErrorHandlingSetup
{
    public static IServiceCollection AddErrorHandling(
        this IServiceCollection services)
    {
        services.AddScoped<IErrorHandler, GlobalErrorHandler>();
        services.AddScoped<EnhancedErrorBoundary>();
        
        services.AddHttpClient("API")
            .AddHttpMessageHandler<EnhancedErrorHandler>()
            .AddHttpMessageHandler<RetryHandler>();
            
        return services;
    }
}