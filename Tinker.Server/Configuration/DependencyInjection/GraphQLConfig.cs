using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Execution.Instrumentation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tinker.Server.GraphQL.Mutations;
using Tinker.Server.GraphQL.Queries;
using Tinker.Server.GraphQL.Types;

namespace Tinker.Server.Configuration.DependencyInjection;

public static class GraphQLConfig
{
    public static IServiceCollection AddGraphQLServices(this IServiceCollection services)
    {
        services
                    .AddAuthorization()
                    .AddGraphQLServer()
                    .AddQueryType(d => d.Name("Query"))
                    .AddMutationType(d => d.Name("Mutation"))
                    .AddTypeExtension<ProductQueries>()
                    .AddTypeExtension<ProductMutations>()
                    .AddTypeExtension<OrderQueries>()
                    .AddTypeExtension<OrderMutations>()
                    .AddType<ProductType>()
                    .AddType<OrderType>()
                    .AddFiltering()
                    .AddSorting()
                    .AddProjections()
                    .AddMaxExecutionDepthRule(10)
                    .ModifyRequestOptions(opt => 
                    {
                        opt.IncludeExceptionDetails = false;
                        opt.ExecutionTimeout = TimeSpan.FromSeconds(30);
                    })
                    .AddInMemorySubscriptions()
                    .AddDiagnosticEventListener<CustomGraphQLDiagnosticEventListener>()
                    .UseDefaultPipeline();

        return services;
    }

    public static IApplicationBuilder UseGraphQLServices(this IApplicationBuilder app)
    {
        return app
            .UseRouting()
            .UseWebSockets()
            .UseEndpoints(endpoints => 
            { 
                endpoints.MapGraphQL()
                    .WithOptions(new GraphQLServerOptions
                    {
                        Tool = { Enable = true },
                        EnableSchemaRequests = true,
                        EnableGetRequests = true
                    });
            });
    }
}

internal class CustomGraphQLDiagnosticEventListener(ILogger<CustomGraphQLDiagnosticEventListener> logger) 
    : ExecutionDiagnosticEventListener
{
    public override void RequestError(IRequestContext context, Exception exception)
    {
        logger.LogError(exception, "GraphQL execution error: {Message}", exception.Message);
    }
}