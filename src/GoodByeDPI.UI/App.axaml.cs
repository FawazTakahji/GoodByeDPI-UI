using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GoodByeDPI.Core;
using GoodByeDPI.Core.Services;
using GoodByeDPI.Core.ViewModels;
using GoodByeDPI.UI.Services;
using GoodByeDPI.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Icon = MsBox.Avalonia.Enums.Icon;

namespace GoodByeDPI.UI;

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
            if (!Environment.IsPrivilegedProcess)
            {
                AdminWarn(desktop);
                return;
            }

            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            IServiceCollection collection = new ServiceCollection();
            collection.AddSingleton(typeof(IArgsProvider), new ArgsProvider(desktop.Args));
            collection.AddSingleton(typeof(IMessageBoxService), new MessageBoxService(desktop));
            Singletons.BuildProvider(collection);

            this.DataContext = Singletons.ServiceProvider.GetRequiredService<AppViewModel>();
            BindingPlugins.DataValidators.RemoveAt(0);

            if (desktop.Args is not {} args || !args.Any(s => s.Equals("--start-minimized", StringComparison.OrdinalIgnoreCase)))
            {
                desktop.MainWindow = new MainWindow(true)
                {
                    DataContext = Singletons.ServiceProvider.GetRequiredService<MainWindowViewModel>()
                };
            }

            desktop.Exit += DesktopOnExit;
        }
#if DEBUG
        else if (Design.IsDesignMode)
        {

        }
#endif
        else
        {
            DesktopWarn();
            return;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static async Task AdminWarn(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var messagebox = MessageBoxManager.GetMessageBoxStandard(Constants.Title,
            "Administrator privileges are required to run this application",
            ButtonEnum.Ok, Icon.Error);
        await messagebox.ShowAsync();
        desktop.Shutdown();
    }

    private static async Task DesktopWarn()
    {
        await MessageBoxManager.GetMessageBoxStandard(Constants.Title,
                "The app only works on desktop.",
                ButtonEnum.Ok,
                Icon.Error)
            .ShowWindowAsync();
        Environment.Exit(0);
    }

    private static void DesktopOnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        if (Singletons.ServiceProvider is not { } provider)
        {
            return;
        }

        provider.GetRequiredService<ProcessManager>().Stop();
    }

    private void SettingsOnClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            return;
        }

        if (desktop.MainWindow is not MainWindow mainWindow)
        {
            desktop.MainWindow = new MainWindow(false)
            {
                DataContext = Singletons.ServiceProvider?.GetRequiredService<MainWindowViewModel>()
            };
        }
        else
        {
            mainWindow.Show();
        }
    }

    private void ExitOnClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void TrayIconClicked(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            return;
        }

        if (desktop.MainWindow is not MainWindow mainWindow)
        {
            desktop.MainWindow = new MainWindow(false)
            {
                DataContext = Singletons.ServiceProvider?.GetRequiredService<MainWindowViewModel>()
            };
        }
        else if (mainWindow.IsVisible)
        {
            mainWindow.Hide();
        }
        else
        {
            mainWindow.Show();
        }
    }
}