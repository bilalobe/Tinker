using Tinker.Infrastructure.Monitoring.Core.Interfaces;

namespace Tinker.Infrastructure.Core.State.Base;

public abstract class StateBase<T> where T : class
{
    protected readonly IMetricsService _metrics;
    protected readonly ILogger _logger;
    
    protected T State { get; private set; }
    
    public event EventHandler<StateChangedEventArgs<T>>? StateChanged;
    
    protected StateBase(T initialState)
    {
        State = initialState;
    }

    protected virtual void OnStateChanged(T oldState, T newState)
    {
        StateChanged?.Invoke(this, new StateChangedEventArgs<T>(oldState, newState));
        _metrics.RecordGauge($"state_change_{typeof(T).Name}", 1);
    }
}