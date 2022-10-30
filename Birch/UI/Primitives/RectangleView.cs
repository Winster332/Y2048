namespace Birch.UI.Primitives;

public abstract class RectangleView : View
{
    public const string WIDTH_FIELD = "width";
    public const string HEIGHT_FIELD = "height";

    public virtual float Width
    {
        get => GetField<float>(WIDTH_FIELD);
        protected set => SetField(WIDTH_FIELD, value);
    }
    public virtual float Height
    {
        get => GetField<float>(HEIGHT_FIELD);
        protected set => SetField(HEIGHT_FIELD, value);
    }

    protected RectangleView()
    {
        RegistryField(WIDTH_FIELD, 0f);
        RegistryField(HEIGHT_FIELD, 0f);
    }

    public override bool IsIntersectWith(float x, float y)
    {
        return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
    }
}
