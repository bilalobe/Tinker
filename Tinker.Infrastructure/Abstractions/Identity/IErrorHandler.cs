using Microsoft.AspNetCore.Diagnostics;
using Tinker.Shared.Exceptions.Common;

namespace Tinker.Infrastructure.Core.Data.Interfaces;

public interface IErrorHandler
{
    Task<ErrorResponse> HandleExceptionAsync(Exception exception, ErrorContext context);
    Task<ErrorDetails> GetErrorDetailsAsync(Exception  exception, bool         includeStackTrace = false);
    bool IsCriticalError(Exception                     exception);
}