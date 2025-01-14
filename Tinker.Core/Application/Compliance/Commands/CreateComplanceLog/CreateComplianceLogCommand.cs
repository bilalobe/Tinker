using GreenDonut;
using MediatR;

namespace Tinker.Core.Application.Compliance.Commands.CreateComplanceLog;

public record CreateComplianceLogCommand : IRequest<Result<>>
{
    public required string Description { get; init; }
}