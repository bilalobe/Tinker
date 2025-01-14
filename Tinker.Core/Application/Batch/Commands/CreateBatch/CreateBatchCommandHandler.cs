using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Batch.ValueObjects;
using Tinker.Core.Services.Batch.Interfaces;

namespace Tinker.Core.Application.Batch.Commands.CreateBatch;

public class CreateBatchCommandHandler(IBatchService batchService, ILogger<CreateBatchCommandHandler> logger)
    : IRequestHandler<CreateBatchCommand, Result<>>
{
    public async Task<Result<>> Handle(CreateBatchCommand request, CancellationToken ct)
    {
        try
        {
            var batch = new Domain.Batch.Entities.Batch(BatchId.New(), request.BatchNumber, request.ExpiryDate);
            await batchService.CreateBatch(batch);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating batch");
            return Result.Failure(ex.Message);
        }
    }
}