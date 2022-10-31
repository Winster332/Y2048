using System.Drawing;
using System.Numerics;
using Birch.Common;
using Birch.Graphics;
using Birch.System;
using Birch.UI.Events.Resolver;
using Birch.UI.Events.Touch;
using Microsoft.Extensions.DependencyInjection;
using Color = Birch.Graphics.Color;

namespace Birch.UI.Screens;

public class StatsControl
{
    private int _fps;
    // private float _memory;
    private List<Vector2> _fpsHistory;
    // private List<float> _memoryHistory;
    private List<float> _fpsBuffer;
    // private List<float> _memoryBuffer;
    private DateTime _dtNextReport;
    private TimeSpan _reportUpdateTimeSpan;
    private int _maxHistoryLength;
    public readonly float Width;
    public readonly float Height;
    private float _maxFps;
    // private float _maxMemory;
    private int _needTakeCount = 21;
    // private Process _process;

    public StatsControl()
    {
        _fps = 0;
        // _memory = 0;
        _maxFps = 60;
        // _maxMemory = GC.GetGCMemoryInfo().TotalCommittedBytes / 1024f / 1024f;
        _fpsHistory = new List<Vector2>();
        // _memoryHistory = new List<float>();
        _fpsBuffer = new List<float>();
        // _memoryBuffer = new List<float>();
        _dtNextReport = DateTime.Now;
        _reportUpdateTimeSpan = TimeSpan.FromSeconds(1);
        _maxHistoryLength = 200;
        Width = 100;
        Height = 25;

        // _process = Process.GetCurrentProcess();

        _fpsHistory = Enumerable.Range(0, _needTakeCount).Select(i => new Vector2(i * 5f, _maxFps)).ToList();
        // _memoryHistory = Enumerable.Range(0, _needTakeCount).Select(_ => _maxMemory).ToList();
    }

    public void Update(float dt, int fps)
    {
        _fps = fps;
        // _memory = _process.PrivateMemorySize64 / 1024f / 1024f;

        // _memoryBuffer.Add(_memory);
        _fpsBuffer.Add(_fps);

        var now = DateTime.Now;
        if (now >= _dtNextReport)
        {
            _dtNextReport = now.Add(_reportUpdateTimeSpan);

            _fpsHistory.Add(GetFpsPoint(_fpsBuffer.Average()));
            // _memoryHistory.Add(_memoryBuffer.Average());

            // if (_memoryHistory.Count > _maxHistoryLength)
                // _memoryHistory.RemoveAt(0);

            _fpsBuffer.Clear();
            // _memoryBuffer.Clear();
        }

        for (var i = 0; i < _fpsHistory.Count; i++)
        {
            _fpsHistory[i] = new Vector2(_fpsHistory[i].X-(0.1f * (dt)), _fpsHistory[i].Y);

            if (_fpsHistory[i].X < -10f || _fpsHistory.Count > _maxHistoryLength) _fpsHistory.RemoveAt(0);
        }
    }

    private Vector2 GetFpsPoint(float fps)
    {
        var k = 100f / _maxFps;
        var pl = Height / 100f;

        return new Vector2(Width+5f, Height - pl * (fps * k));
    }

    public void Draw(IGraphics g)
    {
        g.ClipRect(new Rect(0, 0, Width, Height));
        DrawFpsLine(g);
        // DrawMemoryLine(canvas);

        g.DrawText($"{_fps}", 5, 10, new Paint
        {
            IsAntialias = true,
            TextSize = 14,
            Style = PaintStyle.Fill,
            Color = new Color(150, 150, 150, 100)
        });
    }

    private void DrawFpsLine(IGraphics g)
    {
        // var kl = _fpsHistory.TakeLast(_needTakeCount).ToArray();
        // var k = 100f / _maxFps;
        var path = new SvgPath();
        // var pl = Height / 100f;
        // var points = kl.Select((v, i) =>
        // {
        //     return new SKPoint(i * 5f, Height-pl * (v * k));
        // }).ToArray();
        path.AddPoly(_fpsHistory.Select(x => new Point((int)x.X, (int)x.Y)).ToArray(), false);
        g.DrawPath(path, new Paint
        {
            IsAntialias = true,
            Color = new Color(191,112,55),
            Style = PaintStyle.Stroke,
            StrokeWidth = 1
        });
    }

    private void DrawFpsLine1(IGraphics canvas)
    {
        // var kl = _fpsHistory.TakeLast(_needTakeCount).ToArray();
        // var k = 100f / _maxFps;
        // var path = new SKPath();
        // var pl = Height / 100f;
        // var points = kl.Select((v, i) =>
        // {
        //     // return new SKPoint(i * 5f, MathF.Min(30, MathF.Max(1, pl * (v * k) - 30f)));
        //     return new SKPoint(i * 5f, Height-pl * (v * k));
        // }).ToArray();
        // path.AddPoly(points, false);
        // canvas.DrawPath(path, new SKPaint
        // {
        //     IsAntialias = true,
        //     Color = new SKColor(191,112,55),
        //     Style = SKPaintStyle.Stroke,
        //     StrokeWidth = 1
        // });
    }

    private void DrawMemoryLine(IGraphics canvas)
    {
        // var kl = _memoryHistory.TakeLast(_needTakeCount).ToArray();
        // var k = 100f / _maxMemory;
        // var path = new SKPath();
        // var pl = _height / 100f;
        // var points = kl.Select((v, i) =>
        // {
        //     return new SKPoint(i * 5f, _height-pl * (v * k));
        // }).ToArray();
        // path.AddPoly(points, false);
        // canvas.DrawPath(path, new SKPaint
        // {
        //     IsAntialias = true,
        //     Color = new SKColor(55,156,191),
        //     Style = SKPaintStyle.Stroke,
        //     StrokeWidth = 1
        // });
    }
}

public class ScreenService
{
    private Screen? _screen;
    private IServiceProvider _serviceProvider;
    private IAppContext _appContext;
    private EventResolver _eventResolver;
    private DeltaTime _deltaTime;
    private StatsControl _stats;

    public ScreenService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _appContext = serviceProvider.GetService<IAppContext>()!;
        _eventResolver = new EventResolver();
        _deltaTime = new DeltaTime();
        _screen = null;
        _stats = new StatsControl();
    }

    public void SetScreen<T>() where T : Screen
    {
        _screen = null;
        _screen = (Screen) _serviceProvider.GetService(typeof(T))!;
        // screen.SetSize(Size);
        _screen.Initialize(_appContext);
    }

    public void Update()
    {
        if (_screen == null) return;

        _deltaTime.Update();
        _eventResolver.Invoke();
        var dt = _deltaTime.GetDeltaTime();
        _screen.InvokeUpdate(dt);
        _stats.Update(dt, _deltaTime.GetFPS());
    }

    public void Draw(IGraphics g)
    {
        g.Clear(new Color(3,14,27));

        _screen?.InvokeDraw(g);

        if (_screen != null)
        {
            g.Save();
            g.Translate(20, _screen.Height-_stats.Height-200f);
            _stats.Draw(g);
            g.Restore();
        }
    }

    public void InvokeTouchDown(TouchState state)
        => _eventResolver.AddTouchDown(state, e => _screen?.InvokeTouchDown(e));
    public void InvokeTouchMove(TouchState state)
        => _eventResolver.AddTouchMove(state, e => _screen?.InvokeTouchMove(e));
    public void InvokeTouchUp(TouchState state)
        => _eventResolver.AddTouchUp(state, e => _screen?.InvokeTouchUp(e));
}