using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.Services;
using GoodByeDPI.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GoodByeDPI.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        IServiceProvider services = new ServiceCollection()
            .AddCoreServices()
            .BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            NavigationService navigation = services.GetRequiredService<NavigationService>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = navigation
            };

            navigation.NavigateTo<MainViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}