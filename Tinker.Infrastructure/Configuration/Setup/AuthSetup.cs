using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tinker.Infrastructure.Configuration.Groups.Auth;
using Tinker.Infrastructure.Core.Data.Context;
using Tinker.Infrastructure.Identity.Core.Models;

namespace Tinker.Infrastructure.Configuration.Setup;

public static class AuthSetup
{
    public static IServiceCollection AddAuthSetup(
        this IServiceCollection services,
        AuthSettings settings)
    {
        // Identity Configuration
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = settings.Password.RequiredLength;
            options.Password.RequireDigit = settings.Password.RequireDigit;
            options.Password.RequireLowercase = settings.Password.RequireLowercase;
            options.Password.RequireUppercase = settings.Password.RequireUppercase;
            options.Password.RequireNonAlphanumeric = settings.Password.RequireNonAlphanumeric;
            
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(settings.Lockout.DefaultLockoutTimeSpanMinutes);
            options.Lockout.MaxFailedAccessAttempts = settings.Lockout.MaxFailedAccessAttempts;
            
            options.SignIn.RequireConfirmedEmail = settings.SignIn.RequireConfirmedEmail;
            options.SignIn.RequireConfirmedAccount = settings.SignIn.RequireConfirmedAccount;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
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

            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.IncludeErrorDetails = false;
        });

        return services;
    }
}