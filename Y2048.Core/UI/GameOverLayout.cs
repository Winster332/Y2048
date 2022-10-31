using Birch.FileSystem;
using Birch.Graphics;
using Birch.UI;
using Birch.UI.Primitives;
using Birch.UI.Screens;

namespace Y2048.Core.UI;

public sealed class RectButton : RectangleView
{
    public string Text { get; private set; }
    private Paint _burronPaint;
    private Paint _textPaint;

    public RectButton(string text, float width = 80, float height = 35)
    {
        Width = width;
        Height = height;
        Text = text;

        _burronPaint = new Paint
        {
            Style = PaintStyle.Fill,
            Color = new Color(90, 203, 135),
        };
        _textPaint = new Paint
        {
            Style = PaintStyle.Fill,
            TextAlign = TextAlign.Center,
            Color = new Color(248, 248, 248),
            TextSize = 16
        };

        Draw += OnDraw;
    }

    private void OnDraw(IGraphics g)
    {
        g.Save();
        g.Translate(X, Y);
        g.DrawRoundRect(0, 0, Width, Height, 10, 10, _burronPaint);
        g.DrawText(Text, Width/2, Height/2-3, _textPaint);
        g.Restore();
    }
}

public class GameOverLayout : RectangleView
{
    private Screen _screen;
    private Bitmap _bitmap;
    public string CurrentBalls { get; private set; }
    public string MaxBalls { get; private set; }
    public RectButton PlayButton { get; private set; }
    public event EventHandler<object>? Restart;

    public GameOverLayout(Screen screen, IFileSystem fileSystem)
    {
        _screen = screen;
        _bitmap = new Bitmap(fileSystem.ReadBytesFromAsset("trophy.png"), 25f, 30f);
        CurrentBalls = "0";
        MaxBalls = "0";

        RegistryFieldReference(WIDTH_FIELD, () => _screen.Width);
        RegistryFieldReference(HEIGHT_FIELD, () => _screen.Height);
        SetVisible(false);
        Loaded += OnLoaded;

        Draw += OnDraw;
    }

    private void OnLoaded(View view)
    {
        PlayButton = new RectButton("Начать сначала", Width / 2.5f, 80f);
        PlayButton.SetPosition(Width/2-PlayButton.Width/2, Height/2+170-PlayButton.Height/2);
        PlayButton.Click += (_, args) =>
        {
            Restart?.Invoke(this, args);
        };
        AddView(PlayButton);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    public void Show(string currentBalls, string maxBalls)
    {
        CurrentBalls = currentBalls;
        MaxBalls = maxBalls;

        SetVisible(true);
    }

    private void OnDraw(IGraphics g)
    {
        if (!Visible) return;

        g.Save();
        g.Translate(Width/2, Height/2);

        g.DrawRect(-Width/2, -Height/2, Width, Height, new Paint
        {
            Style = PaintStyle.Fill,
            Color = new Color(0, 0, 0, 200),
        });
        g.DrawText("Игра окончена", 0, -Height/2+100, new Paint
        {
            Style = PaintStyle.Fill,
            TextAlign = TextAlign.Center,
            Color = new Color(248,248,248),
            TextSize = 35
        });
        g.DrawText(CurrentBalls, 0, -Height/2+230, new Paint
        {
            Style = PaintStyle.Fill,
            TextAlign = TextAlign.Center,
            TextSize = 60,
            Color = new Color(248,248,248),
            BoldText = true
        });

        g.DrawText("наивысший балл", 0, 0, new Paint
        {
            Style = PaintStyle.Fill,
            TextAlign = TextAlign.Center,
            Color = new Color(248,248,248),
            TextSize = 18
        });
        g.DrawBitmap(_bitmap, -_bitmap.Width/2-30, -_bitmap.Height/2+54);
        g.DrawText(MaxBalls, -10, 50, new Paint
        {
            Style = PaintStyle.Fill,
            TextAlign = TextAlign.Left,
            Color = new Color(248,248,248),
            BoldText = true,
            TextSize = 18
        });

        g.Restore();
    }
}