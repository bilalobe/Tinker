using Tinker.Core.Domain.Common.Interfaces;
using Tinker.Core.Domain.Suppliers.ValueObjects;

namespace Tinker.Core.Domain.Suppliers.Events;

public record ContactDetailsUpdatedEvent(SupplierId SupplierId, string ContactDetails) : IDomainEvent;