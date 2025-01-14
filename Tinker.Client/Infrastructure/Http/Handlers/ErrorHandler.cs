using System.Net;
using System.Net.Http.Json;
using Blazored.Toast.Services;

namespace Tinker.Client.Infrastructure.Http.Handlers;

public class ErrorHandler(IToastService toastService, ILogger<ErrorHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken  cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await HandleErrorResponse(response);
                toastService.ShowError(error);
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed");
            toastService.ShowError("Connection error. Please check your internet connection.");
            throw;
        }
        catch (TaskCanceledException)
        {
            logger.LogWarning("Request timeout");
            toastService.ShowWarning("Request timed out. Please try again.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
            toastService.ShowError("An unexpected error occurred.");
            throw;
        }
    }

    private async Task<string> HandleErrorResponse(HttpResponseMessage response)
    {
        return response.StatusCode switch
               {
                   HttpStatusCode.NotFound => "The requested resource was not found.",
                   HttpStatusCode.Unauthorized => "Please log in to continue.",
                   HttpStatusCode.Forbidden => "You don't have permission to perform this action.",
                   HttpStatusCode.BadRequest => await GetBadRequestMessage(response),
                   HttpStatusCode.InternalServerError => "A server error occurred. Please try again later.",
                   _ => $"An error occurred: {response.StatusCode}"
               };
    }

    private static async Task<string> GetBadRequestMessage(HttpResponseMessage response)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            return error?.Title ?? "Invalid request. Please check your input.";
        }
        catch
        {
            return "Invalid request. Please check your input.";
        }
    }
}