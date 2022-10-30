namespace Birch.Common;

public class Timer
{
    private bool _enabled;
    public TimeSpan Interval { get; private set; }
    public event Action? Elapsed;
    private DateTime _targetDateTime;

    public bool IsEnabled => _enabled;

    public Timer()
    {
        _enabled = false;
        Interval = TimeSpan.Zero;
        Elapsed = null;
        _targetDateTime = DateTime.Now;
    }

    public void SetInterval(TimeSpan interval)
    {
        Interval = interval;
    }

    public void Update()
    {
        if (!_enabled) return;

        if (DateTime.Now >= _targetDateTime)
        {
            _enabled = false;
            Elapsed?.Invoke();
        }
    }

    public void Start()
    {
        _enabled = true;
        _targetDateTime = DateTime.Now.Add(Interval);
    }
}