using System.Numerics;
using Birch.Common;

namespace Y2048.Core.Services;

public class ManipulatorService
{
    public event EventHandler<Vector2>? Moved;
    public event EventHandler<Vector2>? Released;
    private Vector2? _touchDown;
    private Vector2 _offset;
    private MinMax<float> _horizontalLimiter;

    public ManipulatorService()
    {
        _touchDown = null;
        _offset = Vector2.Zero;
        _horizontalLimiter = new MinMax<float>();
    }

    public void SetHorizontalLimit(float min, float max)
    {
        _horizontalLimiter.Set(min, max);
    }

    public void InvokeTouchDown(float x, float y, float offsetX, float offsetY)
    {
        SaveOffset(x, y, offsetX, offsetY);
    }

    private void SaveOffset(float x, float y, float offsetX, float offsetY)
    {
        _touchDown = new Vector2(x, y);
        _offset = _touchDown.Value - new Vector2(offsetX, offsetY);
    }

    public void InvokeTouchMove(float x, float y)
    {
        if (_touchDown != null)
        {
            var targetPosition = ComputePosition(x, y);

            if (targetPosition.X < _horizontalLimiter.Min)
            {
                targetPosition.X = _horizontalLimiter.Min;
                SaveOffset(x, y, _horizontalLimiter.Min, 0);
            }
            else if (targetPosition.X > _horizontalLimiter.Max)
            {
                targetPosition.X = _horizontalLimiter.Max;
                SaveOffset(x, y, _horizontalLimiter.Max, 0);
            }

            Moved?.Invoke(this, targetPosition);
        }
    }

    public void InvokeTouchUp(float x, float y)
    {
        var position = ComputePosition(x, y);

        Moved?.Invoke(this, position);
        Released?.Invoke(this, position);

        _touchDown = null;
    }

    private Vector2 ComputePosition(float tx, float ty) => new Vector2(tx, ty) - _offset;
}
