using Tinker.Infrastructure.Identity.Core.Models;
using Tinker.Infrastructure.Identity.Core.Services;
using Tinker.Infrastructure.Security.Services;

namespace Tinker.Infrastructure.Configuration.Setup;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tinker.Infrastructure.Configuration.Groups.Security;

public static class SecuritySetup
{
    public static IServiceCollection AddSecurityServices(
        this IServiceCollection services,
        SecuritySettings settings)
    {
        settings.Validate();

        services.AddIdentityServices(settings);
        services.AddAuthenticationServices(settings);
        services.AddAuthorizationPolicies();
        services.AddSecurityServices();

        return services;
    }

    private static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        SecuritySettings settings)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = settings.Password.MinimumLength;
            options.Password.RequireDigit = settings.Password.RequireDigits;
            options.Password.RequireLowercase = settings.Password.RequireLowercase;
            options.Password.RequireUppercase = settings.Password.RequireUppercase;
            options.Password.RequireNonAlphanumeric = settings.Password.RequireSpecialCharacters;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(settings.Authentication.LockoutTimeSpanInMinutes);
            options.Lockout.MaxFailedAccessAttempts = settings.Authentication.MaxFailedAccessAttempts;

            options.SignIn.RequireConfirmedEmail = settings.Authentication.RequireUniqueEmail;
        });

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        SecuritySettings settings)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = settings.Jwt.ValidateIssuer,
                    ValidateAudience = settings.Jwt.ValidateAudience,
                    ValidateLifetime = settings.Jwt.ValidateLifetime,
                    ValidateIssuerSigningKey = settings.Jwt.ValidateIssuerSigningKey,
                    ValidIssuer = settings.Jwt.Issuer,
                    ValidAudience = settings.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(settings.Jwt.SecretKey)),
                    ClockSkew = TimeSpan.FromMinutes(settings.Jwt.ClockSkewMinutes)
                };
            });

        return services;
    }

    private static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();
        services.AddScoped<IMfaService, MfaService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }

    private static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireMfa", policy => 
                policy.RequireClaim("mfa_validated", "true"));
            
            options.AddPolicy("AdminOnly", policy => 
                policy.RequireRole("Administrator"));
        });

        return services;
    }
}
