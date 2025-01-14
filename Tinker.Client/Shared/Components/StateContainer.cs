namespace Tinker.Client.Shared.Components;

public interface IStateContainer<T>
{
    T State { get; }
    void SetState(Action<T> updater);
    event Action OnStateChanged;
}

public class StateContainer<T>(T initialState) : IStateContainer<T>
{
    public T State { get; } = initialState;

    public event Action OnStateChanged;

    public void SetState(Action<T> updater)
    {
        updater(State);
        OnStateChanged?.Invoke();
    }
}