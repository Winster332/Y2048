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

namespace Birch.Android;

public class BirchActivity : AppCompatActivity, IStartup
{
    public SKCanvasView CanvasView;
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
        CanvasView = new SKCanvasView(this);
        CanvasView.PaintSurface += CanvasOnPaintSurface;
        layout.AddView(CanvasView);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, AndroidFileSystem>();
        services.AddSingleton<IGraphics>(_graphics);
    }

    private SizeF GetActivitySize()
    {
        var metrics = new DisplayMetrics();
        WindowManager?.DefaultDisplay?.GetRealMetrics(metrics);

        return new SizeF(metrics.WidthPixels, metrics.HeightPixels);
    }

    public void Configure(IAppContext context)
    {
        var size = GetActivitySize();
        _screenService = context.ScreenService;
        context.SetScreenSize(size.Width, size.Height);

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

        TouchState ToTouchState(View.TouchEventArgs e) => new TouchState(new Vec2(e.Event.GetRawX(0), e.Event.GetRawY(0)));
    }

    private void CanvasOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        _graphics.SetCanvas(e.Surface.Canvas);

        _screenService.Update();
        _screenService.Draw(_graphics);

        CanvasView.Invalidate();
    }
}