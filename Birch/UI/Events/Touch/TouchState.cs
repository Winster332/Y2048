using Birch.Graphics;

namespace Birch.UI.Events.Touch;

public class TouchState : IEventState
{
    public Vec2 AbsolutePosition { get; private set; }
    public bool IsStopPropagation { get; private set; }

    public TouchState(Vec2 absolutePosition)
    {
        AbsolutePosition = absolutePosition;
        IsStopPropagation = false;
    }

    public TouchState NewTarget()
    {
        return new TouchState(AbsolutePosition);
    }

    public void StopPropagation()
    {
        IsStopPropagation = true;
    }
}
