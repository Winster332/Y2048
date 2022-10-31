using System.Drawing;
using System.Numerics;

namespace Birch.Graphics;

public struct Color : IEquatable<Color>
{
    private readonly uint color;
    public Color(uint value) => color = value;
    public Color(byte red, byte green, byte blue, byte alpha) => color = (uint) (alpha << 24 | red << 16 | green << 8) | blue;
    public Color(byte red, byte green, byte blue) => color = (uint) (-16777216 | red << 16 | green << 8) | blue;

    public Color WithRed(byte red) => new (red, Green, Blue, Alpha);
    public Color WithGreen(byte green) => new (Red, green, Blue, Alpha);
    public Color WithBlue(byte blue) => new (Red, Green, blue, Alpha);
    public Color WithAlpha(byte alpha) => new (Red, Green, Blue, alpha);

    public byte Alpha => (byte) (color >> 24 & byte.MaxValue);

    public byte Red => (byte) (color >> 16 & byte.MaxValue);

    public byte Green => (byte) (color >> 8 & byte.MaxValue);

    public byte Blue => (byte) (color & byte.MaxValue);

    public bool Equals(Color obj) => (int) obj.color == (int) color;

    public override bool Equals(object? other) => other is Color otherColor && Equals(otherColor);

    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    public static implicit operator Color(uint color) => new(color);
    public static explicit operator uint(Color color) => color.color;

    public override int GetHashCode() => color.GetHashCode();

    public uint GetUint() => color;
}

public enum PaintStyle
{
    Fill,
    Stroke
}

public class PathEffect
{
    protected PathEffect()
    {
    }

    public static PathEffect CreateDash(float[] intervals, float phase)
    {
        return new PathEffectDash(intervals, phase);
    }
}

public class PathEffectDash : PathEffect
{
    public float[] Intervals { get; }
    public float Phase { get; }
    internal PathEffectDash(float[] intervals, float phase)
    {
        Intervals = intervals;
        Phase = phase;
    }
}

public enum TextAlign
{
    Center,
    Left,
    Right
}

public enum ShaderTileMode
{
    Clamp,
    Repeat,
    Mirror,
    Decal
}

public class Shader
{
    public static Shader CreateLinearGradient(Vec2 start, Vec2 end, Color[] colors, ShaderTileMode mode)
    {
        return new ShaderLinearGradient(start, end, colors, mode);
    }
}

public class Vec2
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vec2(float x, float y)
    {
        X = x;
        Y = y;
    }
}

public class ShaderLinearGradient : Shader
{
    public Vec2 Start { get; }
    public Vec2 End { get; }
    public Color[] Colors { get; }
    public ShaderTileMode Mode {get;}
    internal ShaderLinearGradient(Vec2 start, Vec2 end, Color[] colors, ShaderTileMode mode)
    {
        Start = start;
        End = end;
        Colors = colors;
        Mode = mode;
    }
}

public class Rect
{
    public float X;
    public float Y;
    public float Width;
    public float Height;
    public Rect(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}

public class Paint
{
    public TextAlign TextAlign { get; set; } = TextAlign.Left;
    public bool BoldText { get; set; } = false;
    public PaintStyle Style { get; set; } = PaintStyle.Stroke;
    public Color Color { get; set; } = new (0, 0, 0);
    public float StrokeWidth { get; set; } = 1f;
    public bool IsAntialias { get; set; } = true;
    public PathEffect? PathEffect { get; set; }
    public float TextSize { get; set; } = 16;
    public Shader? Shader { get; set; }
}

public class Bitmap
{
    public byte[] Bytes { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }

    public Bitmap(Stream stream, float width, float height)
    {
        Width = width;
        Height = height;
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        Bytes = memory.ToArray();
    }

    public Bitmap(byte[] bytes, float width, float height)
    {
        Bytes = bytes;
        Width = width;
        Height = height;
    }
}

public class SvgPath
{
    public Point[] Points { get; private set; }
    public bool Close { get; private set; }
    public void AddPoly(Point[] points, bool close)
    {
        Points = points;
        Close = close;
    }
}

public interface IGraphics
{
    public void Save();
    public void Translate(float x, float y);
    public void Restore();
    public void Clear(Color color);
    public void DrawCircle(float x, float y, float radius, Paint paint);
    public void DrawRect(float x, float y, float width, float height, Paint paint);
    public void DrawRoundRect(float x, float y, float width, float height, float rx, float ry, Paint paint);
    public void DrawLine(float x1, float y1, float x2, float y2, Paint paint);
    public void RotateRadians(float radians);
    public void RotateRadians(float radians, float px, float py);
    public void RotateDegrees(float degrees);
    public void RotateDegrees(float degrees, float px, float py);
    public void Scale(float s);
    public void Scale(float sx, float sy);
    public void Scale(float sx, float sy, float px, float py);
    public void DrawText(string text, float x, float y, Paint paint);
    public void DrawBitmap(Bitmap bitmap, float x, float y, Paint? paint = null);
    public void ClipRect(Rect rect);
    public void DrawPath(SvgPath path, Paint paint);
}