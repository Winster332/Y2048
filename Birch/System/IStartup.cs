using Microsoft.Extensions.DependencyInjection;

namespace Birch.System;

public interface IStartup
{
    public void ConfigureServices(IServiceCollection services);
    public void Configure(IAppContext context);
}