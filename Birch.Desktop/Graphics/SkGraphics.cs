using Birch.Desktop.Extensions;
using Birch.Graphics;
using Birch.System;
using SkiaSharp;
using Bitmap = Birch.Graphics.Bitmap;
using Color = Birch.Graphics.Color;

namespace Birch.Desktop.Graphics;

public class SkGraphics : IGraphics
{
    private SKCanvas? _canvas;
    private IAppContext? _context;
    private float _scale;
    public SkGraphics()
    {
        _canvas = null;
        _context = null;
        _scale = 1f;
    }

    internal void SetContext(IAppContext context)
    {
        _context = context;
    }

    public void SetCanvas(SKCanvas canvas, float scale)
    {
        _scale = scale;
        _canvas = canvas;
    }

    public void Clear(Color color)
    {
        _canvas?.Clear(color.ToSk());
    }

    public void Save()
    {
        _canvas?.Save();
    }

    public void Restore()
    {
        _canvas?.Restore();
    }

    public void Translate(float x, float y)
    {
        _canvas?.Translate(x, y);
    }

    public void DrawCircle(float x, float y, float radius, Paint paint)
    {
        _canvas?.DrawCircle(x, y, radius, paint.ToSk());
    }

    public void DrawRect(float x, float y, float width, float height, Paint paint)
    {
        _canvas?.DrawRect(x, y, width, height, paint.ToSk());
    }

    public void DrawRoundRect(float x, float y, float width, float height, float rx, float ry, Paint paint)
    {
        _canvas?.DrawRoundRect(x, y, width, height, rx, ry, paint.ToSk());
    }

    public void DrawLine(float x1, float y1, float x2, float y2, Paint paint)
    {
        _canvas?.DrawLine(x1, y1, x2, y2, paint.ToSk());
    }

    public void DrawText(string text, float x, float y, Paint paint)
    {
        if (_canvas == null || _context == null)
            return;

        var skPaint = paint.ToSk();
        var bounds = SKRect.Empty;

        skPaint.MeasureText(text, ref bounds);

        // _canvas.Save();
        // var size = _context.GetScreenSize();
        // var scale = size.Width / textWidth;
        // _canvas.Scale(scale);
        skPaint.TextSize *= _scale;

        _canvas?.DrawText(text, x, y-bounds.Top, skPaint);
    }

    public void DrawBitmap(Bitmap bitmap, float x, float y, Paint? paint = null)
    {
        _canvas?.DrawBitmap(bitmap.ToSk(), x, y, paint?.ToSk());
    }

    public void ClipRect(Rect rect)
    {
        _canvas?.ClipRect(new SKRect(rect.X, rect.Y, rect.Width, rect.Height));
    }

    public void DrawPath(SvgPath path, Paint paint)
    {
        var p = new SKPath();
        p.AddPoly(path.Points.Select(x => new SKPoint(x.X, x.Y)).ToArray(), path.Close);
        _canvas?.DrawPath(p, paint.ToSk());
    }

    public void RotateRadians(float radians) => _canvas?.RotateRadians(radians);
    public void RotateRadians(float radians, float px, float py) => _canvas?.RotateRadians(radians, px, py);

    public void RotateDegrees(float degrees) => _canvas?.RotateDegrees(degrees);
    public void RotateDegrees(float degrees, float px, float py) => _canvas?.RotateDegrees(degrees, px, py);
    public void Scale(float s) => _canvas?.Scale(s);
    public void Scale(float sx, float sy) => _canvas?.Scale(sx, sy);
    public void Scale(float sx, float sy, float px, float py) => _canvas?.Scale(sx, sy, px, py);
}