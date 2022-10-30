using Birch.System;
using Birch.UI.Screens;
using Microsoft.Extensions.DependencyInjection;

namespace Birch.Desktop.Extensions;

public class ApplicationDesktopBuilder
{
    private int _width;
    private int _height;
    private Type? _startupType;

    internal ApplicationDesktopBuilder()
    {
        _width = 0;
        _height = 0;
        _startupType = null;
    }

    public ApplicationDesktopBuilder WithSize(int width, int height)
    {
        _width = width;
        _height = height;
        return this;
    }

    public ApplicationDesktopBuilder UseStartup<T>() where T : IStartup
    {
        _startupType = typeof(T);
        return this;
    }

    public void Build()
    {
        if (_startupType == null)
            throw new NullReferenceException();

        var startup = (IStartup) Activator.CreateInstance(_startupType)!;
        var context = new AppDesktopContext();

        var services = new ServiceCollection();
        services.AddSingleton<IAppContext>(context);
        startup.ConfigureServices(services);

        var window = new BirchWindow(_width, _height);
        window.ConfigureServices(services);

        context.Initialize(services.BuildServiceProvider(), window);
        window.Configure(context);
        startup.Configure(context);

        Application.Run(window);
    }
}

public class AppDesktopContext : IAppContext
{
    public IServiceProvider ServiceProvider { get; private set; }
    public ScreenService ScreenService { get; private set; }
    private BirchWindow _window;
    private SizeF _size;

    internal AppDesktopContext()
    {
    }

    internal void Initialize(IServiceProvider serviceProvider, BirchWindow window)
    {
        ServiceProvider = serviceProvider;
        _window = window;
        _size = window.SkControl.Size;
        ScreenService = new ScreenService(serviceProvider);
    }


    public SizeF GetScreenSize()
    {
        return _size;
    }

    public void SetScreenSize(float width, float height)
    {
        _size = new SizeF(width, height);
    }
}

public static class ApplicationBuilderExtensions
{
    public static ApplicationDesktopBuilder CreateDesktop(this ApplicationBuilder builder)
    {
        return new ApplicationDesktopBuilder();
    }
}