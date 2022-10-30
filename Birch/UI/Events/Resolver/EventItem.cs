namespace Birch.UI.Events.Resolver;

public class EventItem<TState> where TState : IEventState
{
    public Action<TState> Action { get; }
    public TState State { get; }
    public DateTime DateTime { get; }

    public EventItem(Action<TState> action, TState state, DateTime dateTime)
    {
        Action = action;
        State = state;
        DateTime = dateTime;
    }
}
