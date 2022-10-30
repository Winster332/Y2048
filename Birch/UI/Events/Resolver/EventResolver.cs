using Birch.UI.Events.Keyboard;
using Birch.UI.Events.Touch;

namespace Birch.UI.Events.Resolver;

public class EventResolver
{
    private EventQueue<TouchState> _touchDown;
    private EventQueue<TouchState> _touchMove;
    private EventQueue<TouchState> _touchUp;
    private EventQueue<KeyboardState> _keyboardDown;
    private EventQueue<KeyboardState> _keyboardUp;
    private EventQueue<KeyboardPress> _keyboardPress;

    public EventResolver()
    {
        _touchDown = new EventQueue<TouchState>();
        _touchMove = new EventQueue<TouchState>();
        _touchUp = new EventQueue<TouchState>();

        _keyboardDown = new EventQueue<KeyboardState>();
        _keyboardUp = new EventQueue<KeyboardState>();
        _keyboardPress = new EventQueue<KeyboardPress>();
    }

    public void AddTouchDown(TouchState state, Action<TouchState> actionEvent) => _touchDown.Enqueue(state, actionEvent);
    public void AddTouchMove(TouchState state, Action<TouchState> actionEvent) => _touchMove.Enqueue(state, actionEvent);
    public void AddTouchUp(TouchState state, Action<TouchState> actionEvent) => _touchUp.Enqueue(state, actionEvent);

    public void AddKeyDown(KeyboardState state, Action<KeyboardState> actionEvent) => _keyboardDown.Enqueue(state, actionEvent);
    public void AddKeyUp(KeyboardState state, Action<KeyboardState> actionEvent) => _keyboardUp.Enqueue(state, actionEvent);
    public void AddKeyPress(KeyboardPress state, Action<KeyboardPress> actionEvent) => _keyboardPress.Enqueue(state, actionEvent);

    public void Invoke()
    {
        var events = _touchDown
            .Take()
            .Concat(_touchMove.Take())
            .Concat(_touchUp.Take())
            .Select(ConvertEvent)
            .Concat(_keyboardDown.Take().Select(ConvertEvent))
            .Concat(_keyboardUp.Take().Select(ConvertEvent))
            .Concat(_keyboardPress.Take().Select(ConvertEvent))
            .OrderBy(x => x.dateTime);

        foreach (var eventItem in events)
        {
            eventItem.action.Invoke();
        }
    }

    private (Action action, DateTime dateTime) ConvertEvent<T>(EventItem<T> eventItem) where T : IEventState
    {
        return (() => eventItem.Action.Invoke(eventItem.State), eventItem.DateTime);
    }
}
