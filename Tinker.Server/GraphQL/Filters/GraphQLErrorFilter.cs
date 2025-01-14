using Tinker.Shared.Exceptions.Common;
using IErrorHandler = Tinker.Shared.Interfaces.IErrorHandler;

namespace Tinker.Server.GraphQL.Filters;

public class GraphQLErrorFilter(IErrorHandler errorHandler, ILogger<GraphQLErrorFilter> logger) : IErrorFilter
{
    private readonly ILogger<GraphQLErrorFilter> _logger = logger;

    public IError OnError(IError error)
    {
        _logger.LogError(error.Exception, "GraphQL Error: {Message}", error.Message);


        return error;
    }

    private IError CreateError(IError error, ErrorDetails errorDetails)
    {
        return error
            .WithMessage(errorDetails.Message)
            .WithCode(errorDetails.Type)
            .WithExtensions(errorDetails.Extensions);
    }
}