using Tinker.Infrastructure.Core.State.Base;
using Tinker.Infrastructure.Core.State.Models;

namespace Tinker.Infrastructure.Core.State.Services;

public class OrderState : StateBase<OrderStateModel>
{
    public OrderState() : base(new OrderStateModel()) { }
}