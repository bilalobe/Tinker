using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Tinker.Core.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IMetricsService _metrics;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        IMetricsService metrics)
    {
        _validators = validators;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            _metrics.IncrementCounter("validation.errors", new[]
            {
                new MetricDimension("request_type", typeof(TRequest).Name)
            });

            _logger.LogWarning(
                "Validation failed for {RequestType}. Errors: {@ValidationErrors}",
                typeof(TRequest).Name, failures);

            throw new ValidationException("One or more validation failures occurred", failures);
        }

        return await next();
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        throw new NotImplementedException();
    }
}