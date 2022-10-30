using Birch.Graphics;

namespace Y2048.Core.Services;

public class StateService
{
    public int Current { get; private set; }
    public string CurrentDisplay { get; private set; }
    public int MaxBalls { get; private set; }
    public string MaxBallsDisplay { get; private set; }

    public StateService()
    {
        Current = 0;
        CurrentDisplay = Current.ToString();

        MaxBalls = 0;
        MaxBallsDisplay = MaxBalls.ToString();
    }

    public void Apply(int number)
    {
        Current += number;
        CurrentDisplay = Current.ToString();

        if (Current > MaxBalls)
        {
            MaxBalls = Current;
            MaxBallsDisplay = MaxBalls.ToString();
        }
    }

    public void DrawGameCurrent(IGraphics g)
    {
        g.DrawText(CurrentDisplay, 0, 0, new Paint
        {
            TextSize = 30,
            BoldText = true,
            Color = new Color(248,248,248),
            Style = PaintStyle.Fill,
            TextAlign = TextAlign.Right
        });
    }

    public void Reset()
    {
        Current = 0;
        CurrentDisplay = Current.ToString();
    }

    public void SetMaxBalls(int value)
    {
        MaxBalls = value;
        MaxBallsDisplay = MaxBalls.ToString();
    }
}