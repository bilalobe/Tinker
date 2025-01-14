using Tinker.Core.Domain.Common.Interfaces;

namespace Tinker.Core.Domain.Batch.Events;

public record BatchCreatedEvent(Batch Batch) : IDomainEvent;