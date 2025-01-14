// Application/Commands/ProcessCheckoutCommandHandler.cs

using GreenDonut;
using MediatR;
using Tinker.Core.Domain.Checkout.Aggregates;
using Tinker.Core.Domain.Customers.ValueObjects;
using Tinker.Core.Domain.Orders.Repositories;
using Tinker.Core.Services.Checkout.Interfaces;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Application.Checkout.Commands.ProcessCheckout;

public class ProcessCheckoutCommandHandler : IRequestHandler<ProcessCheckoutCommand, Result<OrderDto>>
{
    private readonly ICheckoutService _checkoutService;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<OrderDto>> Handle(
        ProcessCheckoutCommand request,
        CancellationToken      cancellationToken)
    {
        try
        {
            // Create order aggregate
            var orderAggregate = OrderAggregate.CreateOrder(
                new CustomerId(request.Order.CustomerId),
                request.Order.Items.Select(i => new OrderItem( /*...*/)));

            // Process checkout
            await _checkoutService.ProcessCheckout(orderAggregate);

            // Save and dispatch events
            _orderRepository.Add(orderAggregate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _eventDispatcher.DispatchEvents(orderAggregate.DomainEvents);

            return Result.Success(orderAggregate.ToDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<OrderDto>(ex.Message);
        }
    }
}