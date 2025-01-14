using Tinker.Core.Domain.Common.Interfaces;
using Tinker.Core.Domain.Customers.Entities;

namespace Tinker.Core.Domain.Customers.Events;

public record CustomerCreatedEvent(Customer Customer) : IDomainEvent;