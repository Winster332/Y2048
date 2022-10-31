using System.Numerics;
using Birch.Desktop.FileSystem;
using Birch.Desktop.Graphics;
using Birch.FileSystem;
using Birch.Graphics;
using Birch.System;
using Birch.UI.Events.Touch;
using Birch.UI.Screens;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp.Views.Desktop;

namespace Birch.Desktop;

public class BirchWindow : Form, IStartup
{
    public SKGLControl SkControl { get; }
    private ScreenService _screenService;
    private SkGraphics _graphics;
    private bool _closing = false;

    public BirchWindow(int width, int height)
    {
        Width = width;
        Height = height;
        SkControl = new SKGLControl();
        SkControl.Size = WindowSizeToScreen();
        SkControl.PaintSurface += SkiaControlOnPaintSurface;
        Controls.Add(SkControl);

        _graphics = new SkGraphics();

        Closed += (sender, args) =>
        {
            _closing = true;
        };
    }

    private Size WindowSizeToScreen() => new Size(Size.Width - 16, Size.Height - 39);

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, DesktopFileSystem>();
        services.AddSingleton<IGraphics>(_graphics);
    }

    public void Configure(IAppContext context)
    {
        _screenService = context.ScreenService;
        var currentSize = SkControl.Size - new Size(10, 5);
        context.SetScreenSize(currentSize.Width, currentSize.Height);
        _graphics.SetContext(context);
        Resize += (_, _) =>
        {
            SkControl.Size = WindowSizeToScreen();
            var cs = SkControl.Size - new Size(16, 39);
            context.SetScreenSize(cs.Width, cs.Height);
        };

        SkControl.MouseDown += (_, e) => _screenService.InvokeTouchDown(ToTouchState(e));
        SkControl.MouseMove += (_, e) => _screenService.InvokeTouchMove(ToTouchState(e));
        SkControl.MouseUp   += (_, e) => _screenService.InvokeTouchUp  (ToTouchState(e));

        var thread = new Thread(UpdateScreens);
        thread.Start();

        TouchState ToTouchState(MouseEventArgs e) => new TouchState(new Vec2(e.X, e.Y));
    }

    private int _sleepTimeout = 60;

    private void UpdateScreens()
    {
        while (!_closing)
        {
            _screenService.Update();
            Thread.Sleep(_sleepTimeout);
        }
    }

    private void SkiaControlOnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        // var scale = (float)e.Info.Height / (float)SkControl.Height;
        var scale = (float)e.Info.Height / (float)e.Info.Width;
        _graphics.SetCanvas(e.Surface.Canvas, scale);

        _screenService.Update();
        _screenService.Draw(_graphics);

        SkControl.Invalidate();
    }
}