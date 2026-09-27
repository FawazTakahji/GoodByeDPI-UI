using System;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using GoodByeDPI.Core.Processes;
using H.NotifyIcon.Core;

namespace GoodByeDPI.Avalonia.Tray;

public class TrayIconService
{
    private readonly GoodByeDpiService _service;

    private TrayIconWithContextMenu? _trayIcon;
    private Icon? _trayRunningIcon;
    private Icon? _trayOffIcon;
    private PopupMenuItem? _trayToggleItem;

    private Window? _window;
    private Action? _exit;

    public TrayIconService(GoodByeDpiService service)
    {
        _service = service;
    }

    public void Create(Window window, Action exit)
    {
        _window = window;
        _exit = exit;

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

    public void Dispose()
    {
        _trayIcon?.Dispose();
    }

    private void OnServiceStateChanged(object? sender, GoodByeDpiStateChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(() => UpdateTray(e.IsRunning, e.IsBusy));
    }

    private void UpdateTray(bool isRunning, bool isBusy)
    {
        if (_trayIcon is null || _trayToggleItem is null || _trayRunningIcon is null || _trayOffIcon is null)
        {
            return;
        }

        _trayIcon.UpdateIcon(isRunning ? _trayRunningIcon.Handle : _trayOffIcon.Handle);
        _trayToggleItem.Text = isRunning ? "Stop" : "Start";
        _trayToggleItem.Enabled = !isBusy;
    }

    private void ToggleWindow()
    {
        if (_window is null)
        {
            return;
        }

        if (_window.IsVisible)
        {
            _window.Hide();
        }
        else
        {
            _window.Show();
            _window.Activate();
        }
    }
}