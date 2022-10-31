using System;
using System.Threading;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Birch.FileSystem;
using Birch.Graphics;
using Birch.System;
using Birch.UI.Events.Touch;
using Birch.UI.Screens;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using SkiaSharp.Views.Android;
using NotImplementedException = System.NotImplementedException;

namespace Birch.Android;

public class BirchActivity : AppCompatActivity, IStartup
{
    public SKGLSurfaceView CanvasView;
    private SkGraphics _graphics;
    private ScreenService _screenService;

    protected BirchActivity()
    {
        _graphics = new SkGraphics();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
    }

    public void InitializeBirchEngine(RelativeLayout layout)
    {
        CanvasView = new SKGLSurfaceView(this);
        // CanvasView = new SKCanvasView(this);
        CanvasView.PaintSurface += CanvasViewOnPaintSurface;
        layout.AddView(CanvasView);

        // var thread = new Thread(Update);
        // thread.Start();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, AndroidFileSystem>();
        services.AddSingleton<IGraphics>(_graphics);
        services.AddSingleton(Assets!);
    }

    private SizeF GetActivitySize()
    {
        // var metrics = new DisplayMetrics();
        // WindowManager?.DefaultDisplay?.GetRealMetrics(metrics);
        //
        // return new SizeF(metrics.WidthPixels, metrics.HeightPixels);
        return new SizeF(CanvasView.Width, CanvasView.Height);
    }

    private IAppContext _context;

    public void Configure(IAppContext context)
    {
        _context = context;
        var size = GetActivitySize();
        _screenService = _context.ScreenService;
        _context.SetScreenSize(size.Width, size.Height);
        _graphics.SetContext(_context);

        // Resize += (_, _) =>
        // {
        //     SkControl.Size = WindowSizeToScreen();
        //     context.SetScreenSize(SkControl.Size - new Size(16, 39));
        // };


        CanvasView.Touch += (sender, e) =>
        {
            if (e.Event == null) return;

            if (e.Event.Action == MotionEventActions.Down)
                _screenService.InvokeTouchDown(ToTouchState(e));
            else if (e.Event.Action == MotionEventActions.Move)
                _screenService.InvokeTouchMove(ToTouchState(e));
            else if (e.Event.Action == MotionEventActions.Up)
                _screenService.InvokeTouchUp(ToTouchState(e));
        };

        var metrics = new DisplayMetrics();
        WindowManager?.DefaultDisplay?.GetRealMetrics(metrics);

        _size = new SizeF(metrics.WidthPixels, metrics.HeightPixels);

        TouchState ToTouchState(View.TouchEventArgs e) =>
            new TouchState(new Vec2(e.Event.GetRawX(0), e.Event.GetRawY(0)));
    }

    private SizeF _size;
    private int _sleepTimeout = 20;

    private void Update()
    {
        while (true)
        {
            _screenService.Update();
            Thread.Sleep(_sleepTimeout);
        }
    }

    private void CanvasViewOnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
    {
        // var scale = (float)_size.Height / (float)e.Info.Height;
        var scale = (float)e.Info.Height / (float)e.Info.Width;
        _graphics.SetCanvas(e.Surface.Canvas, scale);

        _screenService.Update();
        _screenService.Draw(_graphics);

        CanvasView.Invalidate();
    }
}