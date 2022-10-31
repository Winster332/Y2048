using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Birch.System;
using Birch.UI.Screens;
using Microsoft.Extensions.DependencyInjection;
using SizeF = System.Drawing.SizeF;

namespace Birch.Android.Extensions;

public class ApplicationAndroidBuilder
{
    private Type? _startupType;

    internal ApplicationAndroidBuilder()
    {
        _startupType = null;
    }

    public ApplicationAndroidBuilder UseStartup<T>() where T : IStartup
    {
        _startupType = typeof(T);
        return this;
    }

    public void Build(BirchActivity activity)
    {
        if (_startupType == null)
            throw new NullReferenceException();

        var startup = (IStartup) Activator.CreateInstance(_startupType)!;
        var context = new AppAndroidContext();

        var services = new ServiceCollection();
        services.AddSingleton<IAppContext>(context);
        startup.ConfigureServices(services);

        activity.ConfigureServices(services);

        context.Initialize(services.BuildServiceProvider(), activity);
        activity.Configure(context);
        startup.Configure(context);
    }
}

public class AppAndroidContext : IAppContext
{
    public IServiceProvider ServiceProvider { get; private set; }
    public ScreenService ScreenService { get; private set; }
    private BirchActivity _activity;
    private SizeF _size;

    internal AppAndroidContext()
    {
    }

    internal void Initialize(IServiceProvider serviceProvider, BirchActivity activity)
    {
        ServiceProvider = serviceProvider;
        _activity = activity;

        // var metrics = new DisplayMetrics();
        // _activity.WindowManager?.DefaultDisplay?.GetRealMetrics(metrics);

        // _size = new SizeF(metrics.WidthPixels, metrics.HeightPixels);
        _size = new SizeF(activity.CanvasView.Width, activity.CanvasView.Height);

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
    public static ApplicationAndroidBuilder CreateAndroid(this ApplicationBuilder builder)
    {
        return new ApplicationAndroidBuilder();
    }
}