using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;
using Serilog;
using Tinker.Infrastructure.Security.Configuration;

namespace Tinker.Server.Setup;

public static class PipelineSetup
{
    public static IApplicationBuilder UseApplicationPipeline(this IApplicationBuilder app)
    {
        return app.UseRouting()
                 .UseHttpsRedirection()
                 .UseSecurityHeaders()
                 .UseStaticFiles()
                 .UseResponseCompression()
                 .UseIpRateLimiting();
    }

    public static IApplicationBuilder UseSecurityPipeline(this IApplicationBuilder app)
    {
        return app.UseSecurityHeaders()
                 .UseHttpsRedirection()
                 .UseIpSafeList()
                 .UseRateLimiting()
                 .UseAntiforgery()
                 .UseAuthentication()
                 .UseAuthorization()
                 .UseSecurityAuditLogging();
    }

    public static IApplicationBuilder UseMonitoringPipeline(this IApplicationBuilder app)
    {
        return app.UseHealthChecks("/health")
                 .UseSerilogRequestLogging()
                 .UseMonitoring(app.ApplicationServices.GetRequiredService<IOptions<AppSettings>>().Value);
    }
}