using System;
using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using GoodByeDPI.Avalonia.Views;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.ViewModels;
using H.NotifyIcon.Core;

namespace GoodByeDPI.Avalonia;

public partial class App : Application
{
    public MainWindow? Window;
    public IClassicDesktopStyleApplicationLifetime? Desktop;

    private TrayIconWithContextMenu? _trayIcon;
    private Icon? _trayRunningIcon;
    private Icon? _trayOffIcon;

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

            desktop.Exit += (_, _) =>
            {
                _trayIcon?.Dispose();
            };

            SetupTray();
        }

        Ioc.Default.GetRequiredService<NavigationService>()
            .NavigateTo<HomeViewModel>();

        base.OnFrameworkInitializationCompleted();
    }

    private void SetupTray()
    {
        _trayRunningIcon = new Icon(AssetLoader.Open(new Uri("avares://GoodByeDPI.Avalonia/Assets/Icons/app.ico")));
        _trayOffIcon = new Icon(AssetLoader.Open(new Uri("avares://GoodByeDPI.Avalonia/Assets/Icons/tray-off.ico")));

        _trayIcon = new TrayIconWithContextMenu
        {
            Icon = _trayOffIcon.Handle,
            ToolTip = "GoodByeDPI UI",
            ContextMenu = new PopupMenu
            {
                Items =
                {
                    new PopupMenuItem("Exit", (_, _) => Dispatcher.UIThread.Post(() => Desktop?.Shutdown())),
                }
            },
        };

        _trayIcon.Create();

        _trayIcon.MessageWindow.MouseEventReceived += (_, e) =>
        {
            if (e.MouseEvent == MouseEvent.IconLeftMouseUp)
            {
                Dispatcher.UIThread.Post(ToggleWindow);
            }
        };
    }

    private void ToggleWindow()
    {
        if (Window is null)
        {
            return;
        }

        if (Window.IsVisible)
        {
            Window.Hide();
        }
        else
        {
            Window.Show();
            Window.Activate();
        }
    }
}