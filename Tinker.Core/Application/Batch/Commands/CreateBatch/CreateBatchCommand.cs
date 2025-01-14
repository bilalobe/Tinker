using GreenDonut;
using MediatR;

namespace Tinker.Core.Application.Batch.Commands.CreateBatch;

public record CreateBatchCommand : IRequest<Result<>>
{
    public required string BatchNumber { get; init; }
    public required DateTime ExpiryDate { get; init; }
}