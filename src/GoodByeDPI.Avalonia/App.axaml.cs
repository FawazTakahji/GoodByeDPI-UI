using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using GoodByeDPI.Avalonia.Views;
using GoodByeDPI.Core.Navigation;
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
            desktop.MainWindow = new MainWindow
            {
                Content = new MainView()
            };
        }

        Ioc.Default.GetRequiredService<NavigationService>()
            .NavigateTo<HomeViewModel>();

        base.OnFrameworkInitializationCompleted();
    }
}