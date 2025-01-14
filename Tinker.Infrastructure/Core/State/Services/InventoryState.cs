using Tinker.Infrastructure.Core.State.Base;
using Tinker.Infrastructure.Core.State.Models;

namespace Tinker.Infrastructure.Core.State.Services;

public class InventoryState : StateBase<InventoryStateModel>
{
    public InventoryState() : base(new InventoryStateModel()) { }
}