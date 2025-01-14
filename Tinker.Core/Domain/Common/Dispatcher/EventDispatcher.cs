using MediatR;
using Tinker.Core.Domain.Common.Interfaces;

namespace Tinker.Core.Domain.Common.Dispatcher;

public class EventDispatcher(IMediator mediator) : IEventDispatcher
{
    public async Task DispatchEvents(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents) await mediator.Publish(domainEvent);
    }
}