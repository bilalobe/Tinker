using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Tinker.Infrastructure.Identity.Core.Interfaces;
using Tinker.Infrastructure.Identity.Core.Services;

namespace Tinker.Infrastructure.Identity.Extensions;

public static class AuthServiceExtensions
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        // JWT configuration
        services.Configure<AuthOptions>(configuration.GetSection("AuthOptions"));

        var authOptions = configuration.GetSection("AuthOptions").Get<AuthOptions>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidAudience = authOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(authOptions.SecretKey)),
                    ClockSkew = TimeSpan.FromMinutes(2),
                    RequireSignedTokens = true,
                    ValidateTokenReplay = true
                };

                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.IncludeErrorDetails = false;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var authService = context.HttpContext.RequestServices
                            .GetRequiredService<IAuthService>();
                        await authService.ValidateTokenAsync(context);
                    },
                    OnAuthenticationFailed = async context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<AuthService>>();
                        logger.LogWarning("Authentication failed: {Error}",
                            context.Exception.Message);
                    }
                };
            });

        return services;
    }
}