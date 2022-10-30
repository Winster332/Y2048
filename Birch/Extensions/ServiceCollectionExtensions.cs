using System.Reflection;
using Birch.UI.Screens;
using Microsoft.Extensions.DependencyInjection;

namespace Birch.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddScreens(this IServiceCollection services, Assembly? assembly)
    {
        if (assembly == null)
            throw new NullReferenceException();

        var screens = assembly.GetTypes()
            .Where(x => x.BaseType == typeof(Screen))
            .ToArray();

        foreach (var screen in screens)
        {
            services.AddTransient(screen);
        }
    }
}