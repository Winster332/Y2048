using Birch.Graphics;
using Birch.Models;
using Birch.Physics;
using Birch.Physics.Shapes;

namespace Y2048.Core.Models;

public class BoundMapModel : GameModel
{
    private float _width;
    private float _heigth;
    public Paint Paint { get; set; }
    public float Thickness { get; set; }


    public BoundMapModel(float width, float height)
    {
        _width = width;
        _heigth = height;
        Thickness = 10f;
        Paint = new Paint
        {
            Style = PaintStyle.Fill,
            Color = new Color(142,138,142)
        };
    }

    protected override Shape[] GetShapes()
    {
        var w = _width;
        var h = 10f;
        var y = _heigth / 2;
        return new Shape[]
        {
            PolygonShape.CreateBox(0, y-h+5, w, h), // bottom
            PolygonShape.CreateBox(-_width/2 + 5, 0, 10, _heigth), // left
            PolygonShape.CreateBox(_width/2 - 5, 0, 10, _heigth), // right
            PolygonShape.CreateBox(0, -_heigth/2, _width, 10), // top
        };
    }

    protected override void Draw(IGraphics g)
    {
        var w = _width;
        var h = 10f;
        var y = _heigth / 2-h/2;
        g.DrawRect(-w / 2, y-5, w, h, Paint);

        g.DrawRect(-_width/2, -_heigth/2, 10,_heigth, Paint);
        g.DrawRect(_width/2-10, -_heigth/2, 10,_heigth, Paint);
        g.DrawRect(-_width/2, -_heigth/2, _width,10, Paint);
    }
}