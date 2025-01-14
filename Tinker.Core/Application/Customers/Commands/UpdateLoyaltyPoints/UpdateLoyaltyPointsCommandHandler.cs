using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Services.Customers.Interfaces;

namespace Tinker.Core.Application.Customers.Commands.UpdateLoyaltyPoints;

public class UpdateLoyaltyPointsCommandHandler(
    ICustomerService                           customerService,
    ILogger<UpdateLoyaltyPointsCommandHandler> logger)
    : IRequestHandler<UpdateLoyaltyPointsCommand, Result<>>
{
    public async Task<Result> Handle(UpdateLoyaltyPointsCommand request, CancellationToken ct)
    {
        try
        {
            await customerService.UpdateLoyaltyPoints(request.CustomerId, request.Points);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating loyalty points for customer {CustomerId}", request.CustomerId);
            return Result.Failure(ex.Message);
        }
    }
}