using Tinker.Shared.Exceptions.Common;
using Tinker.Shared.Models.ErrorHandling;

namespace Tinker.Shared.Components.ErrorHandling;

public interface IErrorHandler
{
    Task HandleErrorAsync(Exception exception, ErrorContext context);
    Task<ErrorResponse> CreateErrorResponseAsync(Exception exception, ErrorContext context);
}