using System;
using System.Drawing;
using Avalonia.Platform;
using Avalonia.Threading;
using GoodByeDPI.Core.Processes;
using GoodByeDPI.Core.Services;
using H.NotifyIcon.Core;

namespace GoodByeDPI.Avalonia.Tray;

public static class TrayManager
{
    private static GoodByeDpiService _service = null!;
    private static IWindowService _window = null!;

    private static TrayIconWithContextMenu? _trayIcon;
    private static Icon? _trayRunningIcon;
    private static Icon? _trayOffIcon;
    private static PopupMenuItem? _trayToggleItem;

    private static Action? _exit;

    public static void Create(Action exit, GoodByeDpiService service, IWindowService window)
    {
        _exit = exit;
        _service = service;
        _window = window;

        _trayRunningIcon = new Icon(AssetLoader.Open(new Uri("avares://GoodByeDPI.Avalonia/Assets/Icons/app.ico")));
        _trayOffIcon = new Icon(AssetLoader.Open(new Uri("avares://GoodByeDPI.Avalonia/Assets/Icons/tray-off.ico")));

        _trayToggleItem = new PopupMenuItem { Text = "Start" };
        _trayToggleItem.Click += (_, _) => Dispatcher.UIThread.InvokeAsync(_service.ToggleAsync);

        _trayIcon = new TrayIconWithContextMenu
        {
            Icon = _trayOffIcon.Handle,
            ToolTip = "GoodByeDPI UI",
            ContextMenu = new PopupMenu
            {
                Items =
                {
                    _trayToggleItem,
                    new PopupMenuSeparator(),
                    new PopupMenuItem("Exit", (_, _) => Dispatcher.UIThread.Post(_exit)),
                },
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

        _service.StateChanged += OnServiceStateChanged;
        UpdateTray(_service.IsRunning, _service.IsBusy);
    }

    public static void ShowNotification(string title, string message, NotificationIcon icon)
    {
        _trayIcon?.ShowNotification(title, message, icon);
    }

    public static void Dispose()
    {
        _trayIcon?.Dispose();
    }

    private static void OnServiceStateChanged(object? sender, GoodByeDpiStateChangedEventArgs e)
    {
        UpdateTray(e.IsRunning, e.IsBusy);
    }

    private static void UpdateTray(bool isRunning, bool isBusy)
    {
        if (_trayIcon is null || _trayToggleItem is null || _trayRunningIcon is null || _trayOffIcon is null)
        {
            return;
        }

        _trayIcon.UpdateIcon(isRunning ? _trayRunningIcon.Handle : _trayOffIcon.Handle);
        _trayToggleItem.Text = isRunning ? "Stop" : "Start";
        _trayToggleItem.Enabled = !isBusy;
    }

    private static void ToggleWindow()
    {
        if (_window.IsVisible)
        {
            _window.Hide();
        }
        else
        {
            _window.Show();
        }
    }
}