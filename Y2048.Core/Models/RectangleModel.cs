using Birch.Graphics;
using Birch.Models;
using Birch.Physics;
using Birch.Physics.Shapes;

namespace Y2048.Core.Models;

public class RectangleModel : GameModel
{
    public float Width { get; set; }
    public float Height { get; set; }
    public Paint Paint { get; set; }

    public RectangleModel()
    {
        Width = 0;
        Height = 0;
        Paint = new Paint
        {
            Style = PaintStyle.Fill,
            Color = new Color(90,203,135)
        };
    }

    protected override Shape[] GetShapes()
    {
        return new Shape[]
        {
            PolygonShape.CreateBox(0, 0, Width, Height),
        };
    }

    protected override void Draw(IGraphics g)
    {
        g.DrawRect(-Width / 2, -Height / 2, Width, Height, Paint);
    }
}
