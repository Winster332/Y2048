using Birch.Common;
using Birch.Graphics;
using Birch.System;
using Birch.UI.Events.Resolver;
using Birch.UI.Events.Touch;
using Microsoft.Extensions.DependencyInjection;

namespace Birch.UI.Screens;

public class ScreenService
{
    private Screen? _screen;
    private IServiceProvider _serviceProvider;
    private IAppContext _appContext;
    private EventResolver _eventResolver;
    private DeltaTime _deltaTime;

    public ScreenService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _appContext = serviceProvider.GetService<IAppContext>()!;
        _eventResolver = new EventResolver();
        _deltaTime = new DeltaTime();
        _screen = null;
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
        // _stats.Update(dt, _deltaTime.GetFPS());
    }

    public void Draw(IGraphics g)
    {
        g.Clear(new Color(3,14,27));

        _screen?.InvokeDraw(g);

        // if (_screen != null)
        // {
            // g.Save();
            // g.Translate(0, _screen.Height-_stats.Height);
            // _stats.Draw(canvas);
            // g.Restore();
        // }
    }

    public void InvokeTouchDown(TouchState state)
        => _eventResolver.AddTouchDown(state, e => _screen?.InvokeTouchDown(e));
    public void InvokeTouchMove(TouchState state)
        => _eventResolver.AddTouchMove(state, e => _screen?.InvokeTouchMove(e));
    public void InvokeTouchUp(TouchState state)
        => _eventResolver.AddTouchUp(state, e => _screen?.InvokeTouchUp(e));
}