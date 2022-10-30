using Birch.Desktop;
using Birch.Desktop.Extensions;
using Birch.System;
using Y2048.Core;

namespace Y2048.Desktop;

static class Program
{
    [STAThread]
    static void Main()
    {
        new ApplicationBuilder()
            .CreateDesktop()
            .WithSize(512, 824)
            .UseStartup<Startup>()
            .Build();
    }
}