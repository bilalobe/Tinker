using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;

namespace Tinker.Infrastructure.Security.Configuration
{
    public static class SecurityHeadersConfig
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(static async (context, next) =>
            {
                // Generate a nonce for Content-Security-Policy
                var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

                // Security headers
                context.Response.Headers["Report-To"] =
                    "{\"group\":\"default\",\"max_age\":31536000,\"endpoints\":[" +
                    "{\"url\":\"/api/security/reports\"}],\"include_subdomains\":true}";

                context.Response.Headers["Content-Security-Policy-Report-Only"] =
                    "report-uri /api/security/reports;";

                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Permissions-Policy"] =
                    "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), " +
                    "microphone=(), payment=(), usb=()";
                context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";
                context.Response.Headers["Expect-CT"] = "max-age=7776000, enforce";

                context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
                context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
                context.Response.Headers["Cross-Origin-Resource-Policy"] = "same-origin";

                context.Response.Headers["Content-Security-Policy"] =
                    $"default-src 'self'; " +
                    $"script-src 'self' 'nonce-{nonce}' 'strict-dynamic'; " +
                    "object-src 'none'; " +
                    "base-uri 'self'; " +
                    "upgrade-insecure-requests; " +
                    "frame-ancestors 'none'; " +
                    "form-action 'self'; " +
                    "connect-src 'self' https:; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self' https:; " +
                    "style-src 'self' 'unsafe-inline' https:; " +
                    "report-uri /api/security/reports;";

                await next();
            });
        }
    }
}