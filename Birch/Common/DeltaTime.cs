namespace Birch.Common;

public class DeltaTime
{
    private long _lastTime = Environment.TickCount;
    private int _fps = 1;
    private int _frames;

    private float _deltaTime = 0.005f;

    public void Update()
    {
        var currentTick = Environment.TickCount;
        if (currentTick - _lastTime >= 1000)
        {
            _fps = _frames;
            _frames = 0;
            _deltaTime = currentTick - _lastTime;
            _lastTime = currentTick;
        }

        _frames++;
    }

    public int GetFPS()
    {
        return _fps;
    }

    public float GetDeltaTime()
    {
        return (_deltaTime / 1000.0f);
    }
}
