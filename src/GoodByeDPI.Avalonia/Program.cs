using Avalonia;
using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using GoodByeDPI.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoodByeDPI.Avalonia;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddCoreServices()
                .BuildServiceProvider());

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
    }
}