using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using GoodByeDPI.Avalonia.Views;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.ViewModels;

namespace GoodByeDPI.Avalonia;

public partial class App : Application
{
    public MainWindow? Window;
    public IClassicDesktopStyleApplicationLifetime? Desktop;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Desktop = desktop;

            Desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Window = new MainWindow
            {
                Content = new MainView()
            };
            desktop.MainWindow = Window;
        }

        Ioc.Default.GetRequiredService<NavigationService>()
            .NavigateTo<HomeViewModel>();

        base.OnFrameworkInitializationCompleted();
    }
}