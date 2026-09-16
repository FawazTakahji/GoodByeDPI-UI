using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
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
            NavigationService navigation = Ioc.Default.GetRequiredService<NavigationService>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = navigation
            };

            navigation.NavigateTo<MainViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}