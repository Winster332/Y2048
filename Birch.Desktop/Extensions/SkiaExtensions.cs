using System.Numerics;
using Birch.Graphics;
using SkiaSharp;
using Bitmap = Birch.Graphics.Bitmap;
using Color = Birch.Graphics.Color;

namespace Birch.Desktop.Extensions;

internal static class SkiaExtensions
{
    public static SKColor ToSk(this Color color)
    {
        return new SKColor(color.GetUint());
    }

    public static SKPathEffect? ToSk(this PathEffect? pathEffect)
    {
        if (pathEffect == null)
            return null;

        var type = pathEffect.GetType();
        if (type == typeof(PathEffectDash))
        {
            var effect = (PathEffectDash)pathEffect;
            return SKPathEffect.CreateDash(effect.Intervals, effect.Phase);
        }

        return null;
    }

    public static SKPoint ToSkPoint(this Vec2 v) => new SKPoint(v.X, v.Y);

    public static SKShaderTileMode ToSk(this ShaderTileMode mode)
    {
        return mode switch
        {
            ShaderTileMode.Clamp => SKShaderTileMode.Clamp,
            ShaderTileMode.Repeat => SKShaderTileMode.Repeat,
            ShaderTileMode.Mirror => SKShaderTileMode.Mirror,
            ShaderTileMode.Decal => SKShaderTileMode.Decal,
            _ => SKShaderTileMode.Clamp
        };
    }

    public static SKShader? ToSk(this Shader? shader)
    {
        if (shader == null)
            return null;

        var type = shader.GetType();

        if (type == typeof(ShaderLinearGradient))
        {
            var linearGradient = (ShaderLinearGradient)shader;
            return SKShader.CreateLinearGradient(
                linearGradient.Start.ToSkPoint(),
                linearGradient.End.ToSkPoint(),
                linearGradient.Colors.Select(x => x.ToSk()).ToArray(),
                linearGradient.Mode.ToSk()
            );
        }

        return null;
    }

    public static SKBitmap ToSk(this Bitmap bitmap)
    {
        return SKBitmap.Decode(bitmap.Bytes).Resize(new SKSizeI((int)bitmap.Width, (int)bitmap.Height), SKFilterQuality.Low);
    }

    public static SKPaint ToSk(this Paint paint)
    {
        return new SKPaint
        {
            TextAlign = paint.TextAlign switch
            {
                TextAlign.Left => SKTextAlign.Left,
                TextAlign.Center => SKTextAlign.Center,
                _ => SKTextAlign.Right
            },
            TextSize = paint.TextSize,
            FakeBoldText = paint.BoldText,
            IsAntialias = paint.IsAntialias,
            Style = paint.Style == PaintStyle.Fill ? SKPaintStyle.Fill : SKPaintStyle.Stroke,
            StrokeWidth = paint.StrokeWidth,
            Color = paint.Color.ToSk(),
            PathEffect = paint.PathEffect?.ToSk(),
            Shader = paint.Shader?.ToSk()
        };
    }
}