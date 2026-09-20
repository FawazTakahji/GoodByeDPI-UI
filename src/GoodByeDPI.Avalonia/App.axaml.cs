using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using GoodByeDPI.Avalonia.Views;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.ViewModels;
using SukiUI.Dialogs;

namespace GoodByeDPI.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        NavigationService navigation = Ioc.Default.GetRequiredService<NavigationService>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                Content = new MainView
                {
                    DataContext = Ioc.Default.GetRequiredService<MainViewModel>(),
                    DialogHost =
                    {
                        Manager = Ioc.Default.GetRequiredService<ISukiDialogManager>()
                    }
                }
            };
        }

        navigation.NavigateTo<HomeViewModel>();

        base.OnFrameworkInitializationCompleted();
    }
}