using Birch.Graphics;

namespace Y2048.Core.Services;

public class DeadLineService
{
    private float _width;
    private float _height;
    private float _mapThickness;
    private Paint _paint;
    private float _countDashes;
    private float _stepWidth;
    private float _y;

    public DeadLineService(float width, float height, float mapThickness)
    {
        _width = width;
        _height = height;
        _mapThickness = mapThickness;

        _countDashes = 12*2f;
        _stepWidth = _width / _countDashes;
        _y = (-_height / 2) + _height / 4f;

        _paint = new Paint
        {
            Color = new Color(98, 92, 98),
            IsAntialias = true,
            StrokeWidth = 2f,
            PathEffect = PathEffect.CreateDash(new float[] { _stepWidth, _stepWidth }, 0)
        };
    }

    public bool PointInDeadArea(float x, float y)
    {
        return y <= _y;
    }

    public void Draw(IGraphics g)
    {
        g.DrawLine((-_width/2) + _stepWidth/2f + _mapThickness, _y, (-_width/2)+_width-_mapThickness, _y, _paint);
    }
}