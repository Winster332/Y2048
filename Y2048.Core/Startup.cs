using System.Reflection;
using Birch.Extensions;
using Birch.Physics;
using Birch.Physics.Box2D;
using Birch.System;
using Microsoft.Extensions.DependencyInjection;
using Y2048.Core.Screens;
using Y2048.Core.Services;

namespace Y2048.Core;

public class Startup : IStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScreens(Assembly.GetExecutingAssembly());
        services.AddTransient<ParticlesService>();
        services.AddTransient<ManipulatorService>();
        services.AddTransient<BoxGeneratorService>();
        services.AddSingleton<StateService>();
        services.AddTransient<IPhysicsService, PhysicsBox2DService>();
    }

    public void Configure(IAppContext context)
    {
        context.ScreenService.SetScreen<GameScreen>();
    }
}