using Tinker.Core.Domain.Common.Interfaces;
using Tinker.Core.Domain.Suppliers.Entities;

namespace Tinker.Core.Domain.Suppliers.Events;

public record SupplierCreatedEvent(Supplier Supplier) : IDomainEvent;