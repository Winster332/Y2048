using System.Numerics;

namespace Birch.Physics.Shapes;

public class CircleShape : Shape
{
    public float Radius { get; private set; }
    public Vector2 Center { get; private set; }

    public CircleShape(float radius)
    {
        Radius = radius;
        Center = Vector2.Zero;
    }

    public override void SetPosition(Vector2 position)
    {
        Center = position;
    }

    public override void SetAngle(float angle, Vector2 position)
    {
    }

    public void SetRadius(float radius)
    {
        Radius = radius;
    }

    public void ScaleRadius(float radius)
    {
        Radius *= radius;
        Center *= radius;
    }

    public override object Clone()
    {
        return new CircleShape(Radius);
    }
}
