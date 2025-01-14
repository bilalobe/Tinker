using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Tinker.Shared.Components.ErrorHandling
{
    public class EnhancedErrorBoundary : ErrorBoundary
    {
        [Inject] private ILogger<EnhancedErrorBoundary> Logger { get; set; } = null!;
        [Inject] private IErrorHandler ErrorHandler { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter] public int MaxRetryAttempts { get; set; } = 1;
        [Parameter] public new RenderFragment? ErrorContent { get; set; }

        private int _retryCount;

        protected override async Task OnErrorAsync(Exception exception)
        {
            if (_retryCount < MaxRetryAttempts)
            {
                _retryCount++;
                Logger.LogWarning(exception, "Retrying after error. Attempt {Attempt}", _retryCount);
                await RecoverAsync();
                return;
            }

            var stackTrace = new StackTrace(exception, true);
            var errorFrame = stackTrace.GetFrames()?.FirstOrDefault();

            Logger.LogError(exception, "UI Error: {Message}", exception.Message);
            
            await ErrorHandler.HandleErrorAsync(exception, new Tinker.Shared.Models.ErrorHandling.ErrorContext
            {
                Component = GetCurrentComponentName(),
                Location = NavigationManager.Uri,
                Timestamp = DateTime.UtcNow,
                Data = new Dictionary<string, string>
                {
                    ["exceptionType"] = exception.GetType().Name,
                    ["message"] = exception.Message,
                    ["stackTrace"] = errorFrame?.ToString() ?? "Unknown",
                    ["retryCount"] = _retryCount.ToString(),
                    ["componentPath"] = errorFrame?.GetFileName() ?? "Unknown"
                }
            });
            
            await base.OnErrorAsync(exception);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (CurrentException is null)
            {
                base.BuildRenderTree(builder);
                return;
            }

            if (ErrorContent is not null)
            {
                builder.AddContent(0, ErrorContent);
                return;
            }

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "error-boundary");
            builder.AddContent(2, $"An error has occurred: {CurrentException.Message}");
            builder.CloseElement();
        }

        private static string GetCurrentComponentName()
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();

            return frames?
                .FirstOrDefault(f => f.GetMethod()?.DeclaringType?.BaseType == typeof(ComponentBase))?
                .GetMethod()?.DeclaringType?.Name ?? "Unknown";
        }

        private async Task RecoverAsync()
        {
            try
            {
                Recover();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Recovery attempt failed");
                await OnErrorAsync(ex);
            }
        }
    }
}