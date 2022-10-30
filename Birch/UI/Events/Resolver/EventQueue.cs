namespace Birch.UI.Events.Resolver;

public class EventQueue<TEventState> where TEventState : IEventState
{
    private List<EventItem<TEventState>> _events;
    private readonly int MaxSize;
    private ReaderWriterLockSlim _locker;

    public EventQueue()
    {
        MaxSize = 50;
        _events = new List<EventItem<TEventState>>();
        _locker = new ReaderWriterLockSlim();
    }

    public void Enqueue(TEventState state, Action<TEventState> eventAction)
    {
        try
        {
            _locker.EnterWriteLock();

            var item = new EventItem<TEventState>(eventAction, state, DateTime.Now);
            _events.Add(item);

            if (_events.Count >= MaxSize)
            {
                _events.RemoveAt(_events.Count - 1);
            }
        }
        finally
        {
            _locker.ExitWriteLock();
        }
    }

    public int Count
    {
        get
        {
            try
            {
                _locker.EnterReadLock();
                return _events.Count;
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }
    }

    public EventItem<TEventState>[] Take()
    {
        try
        {
            _locker.EnterWriteLock();
            // var targetCount = Math.Min(_events.Count, count);

            // if (targetCount == 0) return Array.Empty<EventItem<TEventState>>();
            var list = new List<EventItem<TEventState>>();

            var removeIndexes = new List<int>();
            for (var i = 0; i < _events.Count; i++)
            {
                list.Add(_events[i]);
                removeIndexes.Add(i);
            }

            foreach (var removeIndex in removeIndexes.OrderByDescending(x => x))
            {
                _events.RemoveAt(removeIndex);
            }

            return list.ToArray();
        }
        finally
        {
            _locker.ExitWriteLock();
        }
    }
}
