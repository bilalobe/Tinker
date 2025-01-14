using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace Tinker.Infrastructure.Configuration.Setup;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation
/// </summary>
public static class SwaggerSetup
{
    /// <summary>
    /// Adds Swagger documentation services
    /// </summary>
    public static IServiceCollection AddSwaggerSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSwaggerGen(c =>
        {
            // Add security contact information
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Tinker API",
                Version = "v1",
                Description = "API for Tinker Pharmacy Management System",
                Contact = new OpenApiContact
                {
                    Name = "Security Team",
                    Email = "security@tinker.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Add XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter 'Bearer' followed by space and JWT token"
            });

            // OAuth2 Authentication
            c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                        TokenUrl = new Uri("/connect/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api", "API access" },
                            { "offline_access", "Refresh token" }
                        }
                    }
                }
            });

            // Security Requirements
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Add API versioning support
            c.OperationFilter<SecurityRequirementsOperationFilter>();
            
            // Add request/response examples
            c.ExampleFilters();
            
            // Add schema filters
            c.SchemaFilter<EnumSchemaFilter>();
            c.SchemaFilter<JsonIgnoreSchemaFilter>();
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger UI middleware
    /// </summary>
    public static IApplicationBuilder UseSwaggerSetup(this IApplicationBuilder app)
    {
        app.UseSwagger(c =>
        {
            c.SerializeAsV2 = false;
            c.RouteTemplate = "api-docs/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/api-docs/v1/swagger.json", "Tinker POS API V1");
            options.RoutePrefix = "api/docs";
            options.DocumentTitle = "Tinker POS API Documentation";
            
            // UI Customization
            options.EnableTryItOutByDefault();
            options.EnableFilter();
            options.EnableDeepLinking();
            options.DisplayRequestDuration();
            options.DocExpansion(DocExpansion.None);
            
            // Add custom CSS
            options.InjectStylesheet("/swagger-ui/custom.css");
            
            // OAuth2 Config
            options.OAuthClientId("swagger");
            options.OAuthClientSecret("swagger-secret");
            options.OAuthUsePkce();
        });

        return app;
    }
}