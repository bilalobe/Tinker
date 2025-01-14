using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Compliance.Entities;
using Tinker.Core.Domain.Compliance.ValueObjects;
using Tinker.Core.Services.Compliance.Interfaces;

namespace Tinker.Core.Application.Compliance.Commands.CreateComplanceLog;

public class CreateComplianceLogCommandHandler(
    IComplianceService                         complianceService,
    ILogger<CreateComplianceLogCommandHandler> logger)
    : IRequestHandler<CreateComplianceLogCommand, Result<>>
{
    public async Task<Result> Handle(CreateComplianceLogCommand request, CancellationToken ct)
    {
        try
        {
            var complianceLog = new ComplianceLog(ComplianceLogId.New(), request.Description, DateTime.UtcNow);
            await complianceService.CreateComplianceLog(complianceLog);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating compliance log");
            return Result.Failure(ex.Message);
        }
    }
}