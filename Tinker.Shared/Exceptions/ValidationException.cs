namespace Tinker.Shared.Exceptions;

public class ValidationException(
    string                        message,
    IEnumerable<ValidationError>? errors = null)
    : BusinessException(message,
        "VALIDATION_ERROR",
        new Dictionary<string, object> { ["validationErrors"] = errors ?? Array.Empty<ValidationError>() })
{
    public ValidationException(string property, string error)
        : this("Validation failed", new[] { new ValidationError(property, error) })
    {
    }

    public IReadOnlyList<ValidationError> Errors { get; } = errors?.ToList() ?? new List<ValidationError>();

    public override ExceptionDetails GetDetails()
    {
        return base.GetDetails() with
        {
            Metadata = new Dictionary<string, object>(Metadata)
            {
                ["validationErrors"] = Errors
            }
        };
    }
}