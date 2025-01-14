namespace Tinker.Infrastructure.Core.Data.Interfaces;

public interface IState<out T> where T : class
{
    T Value { get; }
    event Action? OnChange;
    void SetState(Action<T> updater);
}