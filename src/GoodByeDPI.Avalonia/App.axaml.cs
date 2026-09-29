using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using GoodByeDPI.Avalonia.Services;
using GoodByeDPI.Avalonia.Tray;
using GoodByeDPI.Avalonia.Views;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.Processes;
using GoodByeDPI.Core.Services;
using GoodByeDPI.Core.ViewModels;

namespace GoodByeDPI.Avalonia;

public partial class App : Application
{

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            desktop.MainWindow = new MainWindow
            {
                Content = new MainView()
            };

            WindowService.Attach(desktop.MainWindow);

            TrayManager.Create(() => desktop.Shutdown(),
                Ioc.Default.GetRequiredService<GoodByeDpiService>(),
                Ioc.Default.GetRequiredService<IWindowService>());

            desktop.Exit += (_, _) => TrayManager.Dispose();
        }

        Ioc.Default.GetRequiredService<NavigationService>()
            .NavigateTo<HomeViewModel>();

        base.OnFrameworkInitializationCompleted();
    }
}