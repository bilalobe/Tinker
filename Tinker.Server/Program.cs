using System.Configuration;
using AspNetCoreRateLimit;
using Serilog;
using Tinker.Infrastructure;
using Tinker.Infrastructure.Monitoring.Middleware;
using Tinker.Server.Configuration.DependencyInjection;
using QuestPDF.Infrastructure;
using Tinker.Infrastructure.Integration.Http.Handlers;
using Tinker.Infrastructure.Monitoring.Health.Checks;
using Tinker.Infrastructure.Monitoring.Metrics.Collectors;
using Tinker.Shared.Components.ErrorHandling;
using IErrorHandler = HotChocolate.IErrorHandler;

QuestPDF.Settings.License = LicenseType.Community;

// Static Configuration field needed for AddServerServices extension method
public static IConfiguration Configuration { get; private set; } = null!;

var builder = WebApplication.CreateBuilder(args);
Configuration = builder.Configuration;

// Application Configuration
var settings = builder.Configuration
    .GetSection("AppSettings")
    .Get<AppSettings>() ?? throw new InvalidOperationException("AppSettings not configured");

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

// Core Services
builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

// API Services
builder.Services
    .AddGraphQLServices()
    .AddHealthChecks()
    .AddRateLimiting();

// Cross-cutting Concerns
builder.Services
    .AddCacheServices(settings)
    .AddLoggingServices(settings)
    .AddMonitoringServices(settings)
    .AddPerformanceMonitoring(builder.Configuration)
    .AddHostedService<CustomMetricsCollector>()
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<MemoryHealthCheck>("memory")
    .AddCheck<SecurityHealthCheck>("security")
    .AddCheck<BackgroundJobHealthCheck>("background-jobs");

// Security Services
builder.Services
    .AddSecurityServices(builder.Configuration);

// Server Services
builder.Services.AddServerServices();

builder.Services.AddScoped<IErrorHandler, GlobalErrorHandler>();
builder.Services.AddHttpClient("API")
    .AddHttpMessageHandler<EnhancedErrorHandler>()
    .AddHttpMessageHandler<RetryHandler>();

// Add error boundary component
builder.Services.AddScoped<EnhancedErrorBoundary>();

var app = builder.Build();

// Development Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Application Pipeline
app.UseRouting()
    .UseHttpsRedirection()
    .UseSecurityHeaders()
    .UseStaticFiles()
    .UseResponseCompression()
    .UseIpRateLimiting();

// Security Pipeline
app.UseSecurityHeaders()
   .UseHttpsRedirection()
   .UseIpSafeList()
   .UseRateLimiting()
   .UseAntiforgery()
   .UseAuthentication()
   .UseAuthorization()
   .UseSecurityAuditLogging();

// Monitoring Pipeline
app.UseHealthChecks("/health")
    .UseSerilogRequestLogging()
    .UseMonitoring(settings);

// API Endpoints
app.MapGraphQLEndpoint();

app.UseMiddleware<PerformanceTrackingMiddleware>();

await app.RunAsync();

public static IServiceCollection AddServerServices(this IServiceCollection services)
{
    services.AddInfrastructureServices(Configuration);
    services.AddSwaggerServices();
    services.AddGraphQLServices();
    services.AddBackgroundServices();
    return services;
}