using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;

namespace Tinker.Server.Filters;

public class ApiExceptionFilter(
    ILogger<ApiExceptionFilter> logger,
    IMetricsService             metrics)
    : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(
            context.Exception,
            "Unhandled exception occurred while executing {Action}",
            context.ActionDescriptor.DisplayName);

        metrics.IncrementCounter("api.errors");

        var result = new ObjectResult(new
        {
            Type = context.Exception.GetType().Name,
            Title = "An unexpected error occurred",
            Detail = context.Exception.Message,
            TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.Result = result;
    }
}